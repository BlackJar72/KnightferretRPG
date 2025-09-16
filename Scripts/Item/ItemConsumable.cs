using System;
using UnityEngine;


namespace kfutils.rpg
{


    public class ItemConsumable : ItemEquipt, IUsable
    {


        [SerializeField] float useTime;
        [SerializeField] ItemActions useAnimation;


        private bool ready;


        private IActor holder;

        public AbstractAction UseAnimation => throw new System.NotImplementedException();

        public int StaminaCost => throw new System.NotImplementedException();


        public void DecrimentSlot()
        {
            holder.CharInventory.Equipt.RemoveFromSlot((int)prototype.EquiptType, 1);
        }


        public void OnEquipt(IActor actor)
        {
            holder = actor;
            if (equiptAnim != null) PlayEquipAnimation(actor);
            // TODO: Anythings else needed to equiped item?
            throw new System.NotImplementedException();
        }


        public void OnUnequipt()
        {
            if (equiptAnim != null)
            {
                ready = false;
                holder.RemoveEquiptAnimation();
            }
        }


        public void OnUse(IActor actor)
        {
            DecrimentSlot();
            // TODO: Code for actually using item
            throw new System.NotImplementedException();
        }


        public void PlayEquipAnimation(IActor user)
        {
            if (user.ActionState.NormalizedTime >= 1)
            {
                ready = false;
                user.PlayAction(useAnimation.Primary.mask, equiptAnim, OnEqipAnimationEnd, 0);
                user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            }
        }


        public void PlayUseAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
        }

        
        public void OnEqipAnimationEnd()
        {
            ready = true;
        }



    }


}