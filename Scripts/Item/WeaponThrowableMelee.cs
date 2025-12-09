using System;
using UnityEngine;


namespace kfutils.rpg {

    public class WeaponThrowableMelee : WeaponMelee
    {
        [Tooltip("The item's thrown form; its impactPrefab should \nbe a non-tangible version of it's item in world.")] 
        [SerializeField] Projectile projectile;
        [SerializeField] ItemActions throwAnimation;


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
                attackState.Events.SetCallback(0, LaunchWeapon);
                busy = true;
            }
        }


        private void LaunchWeapon()
        {
            Projectile thrown = Instantiate(projectile, transform);
            GameObject thrownObject = thrown.gameObject;
            thrownObject.transform.parent = transform.root.transform;
            thrown.Launch(holder, aim.toward);
            holder.CharInventory.Equipt.ConsumeItem(ItemUtils.GetEquiptSlotForType(prototype.EquiptType), 1);
        }





    }

}
