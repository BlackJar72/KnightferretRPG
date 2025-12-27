using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {


    /// <summary>
    /// Repressent bows, long or short, self or composit. 
    /// 
    /// Bows are the most complex weapon by far, as they require two items, the bow itself 
    /// and the arrow it shoots; the same would apply to other ammo using items, though bows 
    /// are the only such weapon incuded here.  Bows also involve a lot of design decisions.
    /// 
    /// Aim and arching can be handle in several obviouse ways, all of which have been done 
    /// before, probably many times:
    /// 
    /// * Purely physical arrows relying on player skill to aim high enough to hit distant 
    ///   targets (example, Minecraft).  Easy for the dev, requires more player skill.
    /// 
    /// * Partial auto-aim on the vertical so that the player aims directly at the target 
    ///   but the game calculates the angle to needed to hit the target with a physically 
    ///   based arrow (example, Skyrim).  Easy for the player, more challenging for the 
    ///   developer, though there are several exist code assets that can do this for you 
    ///   (something that the previous method needs for NPCs unless they are allowed to 
    ///   cheat).
    /// 
    /// * Weightless arrows that travel in straight line (example, Dark Souls).  This is 
    ///   the easiest for everyone.
    /// 
    /// Then there is the speed of the arrow, for some comparisons Arrow speeds in games (and reality):
    /// 
    ///  * Minecraft: 20 m/s
    ///  * Skyrim: ~24 m/s
    ///  * Caverns of Evil: 54 m/s (fired by goblin enemies)
    ///  * D&D (based on max outdoor range and physics): ~48 m/s for long bow, ~42 m/s for short bow
    ///  * Real medieval war bows: usually between 45 and 61 m/s
    /// 
    /// (Note, arrow speeds for Dark Souls are unkown outside FromSoftware, but guestimated at 5 m/s to 
    /// 10 m/s depending on type; very slow.)
    ///
    /// There is a cost / some problems with the higher speeds.  Special physics settings are needed
    /// to make sure they don't jump past a target between frames (the "bullet through paper" problem). 
    /// Fast arrows are less visible to the player.  Determining hit locations is often inaccurate (I removed 
    /// arrows shown stuck in objects after hitting from Caverns of Evil for that reason, they would end up 
    /// floating in the air far from where they should have hit).
    /// 
    /// Also, many games allow the player to draw the bow, hold, and release when ready; this is a 
    /// layer of complexity most other weapons (basically no melee weapons) have.  This would be 
    /// ideal, though I'd probably want to apply a stamina cost after a few seconds to prevent the 
    /// silliness of walking down the road with a fully drawn bow while still allowing the player to 
    /// adjust their aim before releasing if wanted.  However, not all games allow this and some do
    /// just shoot when the bow is used; I will likely take this approach, and as my arrows are fast 
    /// and aimed directly at whatever is under the crosshairs this seems to work well.  
    /// 
    /// For my game, arrows will likely be fast, comparable to D&D and real life, and travel weightlessly 
    /// in a straight line.  This is different from the slower, thrown weapons and items.  Bows will 
    /// probably fire when used; at least for the first demo game made with hold a bow drawn will not 
    /// be supported.
    /// </summary>
    public class WeaponBow : ItemEquipt, IWeapon
    {

        /// <summary>
        /// Stages for archery if the player is allowed to hold; also include a full complete 
        /// shooting animation as the last entry.  Holding would cost stamina, but I'd prefer 
        /// it be possible.  However, I may start using only implementing a complete shot 
        /// as a single action, as this is simpler to start with.  Perhaps I could add 
        /// more control (holding a shot later).
        /// 
        /// How doable, and how complex, a draw, hold (potentially), shoot system with the 
        /// systems I currently have is.  I may end up with a click to shoot system and no 
        /// holding.  Holding for long periods is realistic, but gamers like being able to
        /// hold to better aim.  However, this may not be a realistic goal with the current 
        /// systems, or at the very least not a good starting point.  
        /// </summary>
        public enum Stages
        {
            ready = 0,
            draw = 1,
            hold = 2,
            loose = 3,
            fullShoot = 4
        }

        // Weapon Fields
        [SerializeField] protected float attackTime;
        [Tooltip ("Defines damage type; the actual damage damage will actually \nbe a bonus and should be low (or zero).")]
        [SerializeField] protected DamageSource damage;

        [Tooltip ("This needs to be an ActionSequence with 5 actions keyed to the stages enum. \n"
                 + " (ready = 0, draw = 1, hold = 2, loose = 3, full shoot sequence = 4)")]
        [SerializeField] protected ItemActions useAnimations; 
        [SerializeField] protected ActionSequence bowAnimations; 
        [SerializeField] protected Transform projectileSpawn; // Should I use this? Or AimParams.from?
        [Tooltip ("Where the arrow should attach and what the \nleft hand fingers should stickt to while drawing.")]
        [SerializeField] protected Transform nock;
        [SerializeField] string ammoTypeID;
        [SerializeField] protected int drawCost;
        [SerializeField] protected int holdCost;
        [SerializeField] protected float lauchSpeed = 48.0f;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected float noise = 4.0f; 
        [SerializeField] protected AnimancerComponent animancer;
        [SerializeField] protected float bowStartTime;
        [SerializeField] protected float bowLooseTime;
        [SerializeField] protected float endAnimTime;

        private readonly WaitForSeconds waitForOnSecond = new(1.0f);

        protected bool busy = true;
        protected bool attacking = true;
        protected ICombatant holder;
        protected AimParams aim;
        protected AnimancerState userState;
        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;
        protected GameObject ammoImage;

        public float GetAttackSpeed() => 1.0f / attackTime;
        public float EstimateDamage(IDamageable victem) => damage.EstimateDamage(victem);
        public int GetDamage() => damage.BaseDamage;

        public bool Parriable => false;
        public float MaxRange => lauchSpeed;
        public float MinRange => 3.0f;
        public AbstractAction UseAnimation => useAnimations.Primary;
        public int StaminaCost => drawCost;
        public int PowerAttackCost => holdCost;
        public float AttackTime => attackTime;
        public override bool IsReal => holder != null;


        private void Awake()
        { 
            bowAnimations = Instantiate(bowAnimations);           
            actionLayer = animancer.Layers[0];
            actionState = actionLayer.Play(bowAnimations.GetSequential((int)Stages.ready));
            float drawTime = bowLooseTime - bowStartTime;
            drawTime = bowAnimations.GetSequential((int)Stages.fullShoot).Clip.length / drawTime;
            bowAnimations.GetSequential((int)Stages.fullShoot).Speed = drawTime;
        }


        public void AttackMelee(ICombatant attacker)
        {
            #if UNITY_EDITOR
            Debug.LogError("Trying to perform melee attack with ranged weapon " + prototype.Name);
            throw new System.NotImplementedException();
            #endif
        }
        public void BeBlocked(ICombatant blocker, BlockArea blockArea) {}


        public void AttackRanged(ICombatant attacker, Vector3 direction)
        {
            LauchProjectile(attacker, direction);
        }


        // FIXME / TODO: Should this be used as a delegate with animator/animancer events?  Or turned into a coroutine and timed?
        public void LauchProjectile(ICombatant attacker, Vector3 direction)
        {
            if(ammoImage != null) Destroy(ammoImage);
            ItemAmmo ammo = attacker.GetAmmoItem();
            Projectile shot = Instantiate(ammo.ShotProjectile, projectileSpawn); // It might be better to use AimParams.from (or might not)  
            shot.transform.position = projectileSpawn.position;
            direction = aim.toward;
                if (Physics.Raycast(aim.from, aim.toward, out RaycastHit hitInfo, shot.Speed, GameConstants.attackableLayer))
                {
                    direction = hitInfo.point - projectileSpawn.transform.position;
                }
                else if (Physics.Raycast(aim.from, aim.toward, out hitInfo, shot.Speed, GameConstants.LevelMask))
                {
                    direction = hitInfo.point - projectileSpawn.transform.position;
                }
            if(shot is ArrowProjectile arrow) 
            {
                arrow.SetSpeed(lauchSpeed - ammo.RangePentalty);
                arrow.GetDamage().SetBaseDamage(CombineDamage(ammo).BaseDamage);  
            }           
            shot.transform.parent = WorldManagement.WorldLogic.GetChunk(transform.position).gameObject.transform;
            shot.transform.LookAt(shot.transform.position + direction); // We will see if this should be plus or minus
            shot.Launch(attacker, direction.normalized);
            ammo.DecrimentSlot();
        }


        public void OnEquipt(IActor actor)
        {
            holder = actor as ICombatant;
            StartCoroutine(WaitUntilEquipt());
            if (actor.ActionState != null) PlayEquipAnimation(actor);
            PCActing pc = actor as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
        }


        private IEnumerator WaitUntilEquipt()
        {
            busy = true;
            yield return waitForOnSecond;
            busy = false;
        }


        public void OnUnequipt()
        {
            holder.RemoveEquiptAnimation();
        }


        public void OnUse(IActor actor)
        {
            if(busy) return;
            ItemAmmo ammo = holder.GetAmmoItem();
            if(IsRightAmmo(ammo)) {
                busy = true;
                PlayUseAnimation(actor);
            }
        }


        protected bool IsRightAmmo(ItemAmmo ammo)
        {
            return (ammo != null) && (string.CompareOrdinal(ammo.ID, ammoTypeID) == 0);
        }
 

        protected void Shoot()
        {
            ItemAmmo ammo = holder.GetAmmoItem();
            // I'd rather just use an ennum for efficient comparison; the cost of separate open and closed source parts.
            if(IsRightAmmo(ammo)) 
            {
                holder.GetAimParams(out aim);
                Vector3 direction = aim.toward;
                // If there is a target close enough to make flat-shooting plausible, try that.
                if (Physics.Raycast(aim.from, aim.toward, out RaycastHit hitInfo, lauchSpeed * 0.25f, GameConstants.attackableLayer | GameConstants.LevelMask))
                {
                    direction = hitInfo.point - projectileSpawn.position;
                }
                // Otherwise, allow arched shooting while correcting the horizontal aim toward a target directing in front of the shooter.
                // I am not sure how to handle shooting at other angles. This is apparently this can be done by using 2D colliders, 
                // but would require all coordinates to be transformed so that the xy plane could represent the worlds xz plane.
                // For now, we are assuming the long range tartet is directly in from of the player.
                //
                // Alternately, the arrow could be spawned directly in front of the player, like a spell (from the spell projectile spawn),
                // which would eliminate this problem but might not look convincing.
                else if (Physics.Raycast(aim.from, holder.GetTransform.forward, out hitInfo, lauchSpeed, GameConstants.attackableLayer | GameConstants.LevelMask))
                {
                    direction.Set(hitInfo.point.x - projectileSpawn.position.x, direction.y, hitInfo.point.z - projectileSpawn.position.z);
                }
                AttackRanged(holder, direction);
            }     
        }


        public void OnUseCharged(IActor actor)
        {
            if(busy) return;
            OnUse(actor);
        }


        public void PlayEquipAnimation(IActor actor)
        {
            if (actor.ActionState.NormalizedTime >= 1)
            {
                actor.PlayAction(useAnimations.Primary.mask, equiptAnim, OnEqipAnimationEnd, 0);
            }
        }


        public void OnEqipAnimationEnd()
        {
            //busy = false;
        }


        public void PlayUseAnimation(IActor actor)
        {
            AbstractAction action;
            if(actor is PCActing) action = useAnimations.Primary;
            else action = useAnimations.Secondary;
            useAnimations.PrimarySound.Play(audioSource);
            userState = actor.PlayAction(useAnimations.Primary.mask, action.GetSequential((int)Stages.fullShoot), OnUseAnimationEnd, 0, attackTime);

            PCActing pc = actor as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.mid);

            GameObject imagePrefab = holder.GetAmmoItem().ImagePrefab;
            if(imagePrefab != null)
            {
                ammoImage = Instantiate(imagePrefab, nock);
                ammoImage.layer = gameObject.layer;
            }
            
            StartCoroutine(DoShootSequence());
            busy = true;
        }


        protected IEnumerator DoShootSequence()
        {
            yield return new WaitForSeconds(bowStartTime);
            OnDrawStart();
            yield return new WaitForSeconds(bowLooseTime - bowStartTime);
            OnArrowLoosed();
            yield return new WaitForSeconds(endAnimTime - bowLooseTime - bowStartTime);
            OnUseAnimationEnd();
            busy = false;
        }


        protected void OnDrawStart()
        {            
            actionState = actionLayer.Play(bowAnimations.GetSequential((int)Stages.fullShoot));
        }


        protected void OnArrowLoosed()
        {
            actionState = actionLayer.Play(bowAnimations.GetSequential((int)Stages.ready));
            Shoot();
        }


        protected void OnUseAnimationEnd()
        {
            //busy = false;
        }


        private DamageSource CombineDamage(ItemAmmo itemAmmo) => damage.Combine(itemAmmo.Damage);



    }

}
