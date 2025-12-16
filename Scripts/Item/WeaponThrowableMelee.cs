using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponThrowableMelee : WeaponMelee
    {
        [Tooltip("The item's thrown form; its impactPrefab should \nbe a non-tangible version of it's item in world.")] 
        [SerializeField] Projectile projectile;
        [SerializeField] ItemActions throwAnimation;
        [SerializeField] float releaseTime = 0.18f;


        private AimParams aim;


        protected override void Awake()
        {
            base.Awake();
            throwAnimation = Instantiate(throwAnimation);
        }



        public override void OnUseCharged(IActor actor)
        {
            if(busy) return;
            if(actor is ICombatant attacker) 
            {
                attacker.GetAimParams(out aim);
                if(Physics.Raycast(aim.from, aim.toward, maxRange, GameConstants.attackableLayer)) base.OnUseCharged(actor);
                else ThrowWeapon(attacker);
                busy = true;
            }
        }


        private void ThrowWeapon(ICombatant attacker)
        {
            if (!busy)
            {
                AbstractAction action;
                action = useAnimation.Primary;
                useAnimation.SecondarySound.Play(audioSource);

                if (attacker is PCActing)
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, action.GetSequential(attack), OnUseAnimationEnd, 0, attackTime);
                }
                else
                {
                    attackState = attacker.PlayAction(useAnimation.Primary.mask, action.GetRandom(attack), OnUseAnimationEnd, 0, attackTime);
                }

                PCActing pc = attacker as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
                busy = true;
                StartCoroutine(DelayedLaunch());
            }
        }


        private IEnumerator DelayedLaunch()
        {
            yield return new WaitForSeconds(releaseTime);
            LaunchWeapon();
        }


        private void LaunchWeapon()
        {
            Vector3 direction = aim.toward;
            Projectile thrown = Instantiate(projectile, transform);
            GameObject thrownObject = thrown.gameObject;
            if(thrown is SpellProjectile spell) spell.SetRange(50, transform.position);
            if(thrown is ThrownWeapon weapon) weapon.SetItem(prototype);
            if(Physics.Raycast(aim.from, aim.toward, out RaycastHit hitInfo, 64, GameConstants.attackableLayer))
            {
                direction = hitInfo.point - thrown.transform.position;
            }     
            thrownObject.transform.parent = WorldManagement.WorldLogic.GetChunk(transform.position).gameObject.transform;
            thrownObject.transform.LookAt(transform.position - direction);
            thrown.Launch(holder, direction.normalized);
            holder.CharInventory.Equipt.ConsumeItem(ItemUtils.GetEquiptSlotForType(prototype.EquiptType), 1);
            attackState.Events.Clear();
        }
    





    }

}
