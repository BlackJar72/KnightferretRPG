using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

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

        protected bool busy = false;
        protected ICombatant holder;
        protected AimParams aim;
        protected AnimancerState userState;
        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;

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
            ItemAmmo ammo = attacker.GetAmmoItem();
            Projectile shot = Instantiate(ammo.ShotProjectile, projectileSpawn); // It might be better to use AimParams.from (or might not)  
            shot.transform.position = aim.from;
            direction = aim.toward;
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
            if (actor.ActionState != null) PlayEquipAnimation(actor);
            PCActing pc = actor as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
        }


        public void OnUnequipt()
        {
            holder.RemoveEquiptAnimation();
        }


        public void OnUse(IActor actor)
        {
            ItemAmmo ammo = holder.GetAmmoItem();
            if((!busy) && IsRightAmmo(ammo)) {
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
            // FIXME: Should be able to hold (though at a cost)
            //        Should deginitely *NOT* autofire after half a second!!!
            OnUse(actor);
        }


        public void PlayEquipAnimation(IActor actor)
        {
            if (actor.ActionState.NormalizedTime >= 1)
            {
                actor.PlayAction(useAnimations.Primary.mask, equiptAnim, OnEqipAnimationEnd, 0);
                busy = true;
                actor.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            }
        }


        public void OnEqipAnimationEnd()
        {
            busy = false;
        }


        public void PlayUseAnimation(IActor actor)
        {
            AbstractAction action = useAnimations.Primary;
            useAnimations.PrimarySound.Play(audioSource);
            userState = actor.PlayAction(useAnimations.Primary.mask, action.GetSequential((int)Stages.fullShoot), OnUseAnimationEnd, 0, attackTime);

            PCActing pc = actor as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
            
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
            busy = false;
        }


        private DamageSource CombineDamage(ItemAmmo itemAmmo) => damage.Combine(itemAmmo.Damage);



    }

}
