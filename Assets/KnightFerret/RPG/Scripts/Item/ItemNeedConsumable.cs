using UnityEngine;


namespace kfutils.rpg
{


    public class ItemNeedConsumable : ItemConsumable
    {

        public override void OnUse(IActor actor)
        {
            DecrimentSlot();
            PlayUseAnimation(actor);
        }


        public override void PlayUseAnimation(IActor actor)
        {
            ready = false;
            if (actor is PCActing)
            {
                PCActing pc = actor as PCActing;
                pc.SetArmsPos(PCActing.ArmsPos.mid);
                holder.PlayAction(useAnimation.mask, useAnimation.anim, OnUseAnimationEnd, 0, useTime);
            }
            else
            {
                holder.PlayAction(useAnimation.mask, useAnimation.anim, OnUseAnimationEnd, 0, useTime);
            }

        }


        private void OnUseAnimationEnd()
        {
            PCActing pc = holder as PCActing;
            if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
            ready = true;
        }
        

    }


}