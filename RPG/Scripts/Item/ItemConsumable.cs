using System;
using UnityEngine;


namespace kfutils.rpg
{


    public abstract class ItemConsumable : ItemEquipt, IUsable
    {

        [SerializeField] protected float useTime;
        [SerializeField] protected AbstractAction useAnimation;

        protected bool ready;

        protected IActor holder;

        public AbstractAction UseAnimation => useAnimation;
        public virtual int StaminaCost => 0;
        public int PowerAttackCost => 0;


        public void DecrimentSlot()
        {
            holder.CharInventory.Equipt.ConsumeItem(ItemUtils.GetEquiptSlotForType(prototype.EquiptType), 1);
        }


        public bool OutOfItem()
        {
            return (holder.CharInventory.Equipt.GetNumberInSlot(ItemUtils.GetEquiptSlotForType(prototype.EquiptType)) < 1);
        }


        public void OnEquipt(IActor actor)
        {
            holder = actor;
            if (equiptAnim != null) PlayEquipAnimation(actor);
            // TODO: Anythings else needed to equiped item?
        }


        public void OnUnequipt()
        {
            ready = false;
            if (equiptAnim != null)
            {
                holder.RemoveEquiptAnimation();
            }
        }


        public void PlayEquipAnimation(IActor user)
        {
            //if (equiptAnim != null)
            //{
            //    ready = false;
            //    user.PlayAction(useAnimation.mask, equiptAnim, OnEqipAnimationEnd, 0);
            //    user.ActionState.Events.OnEnd = OnEqipAnimationEnd;
            //}
            //else
            {
                ready = true;
            }
        }


        public void OnUseCharged(IActor actor)
        {
            OnUse(actor);
        }


        public abstract void OnUse(IActor actor);


        public abstract void PlayUseAnimation(IActor actor);


        public virtual void OnEqipAnimationEnd()
        {
            ready = true;
        }
        

    }


}