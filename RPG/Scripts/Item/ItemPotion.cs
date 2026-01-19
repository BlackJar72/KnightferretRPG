using UnityEngine;


namespace kfutils.rpg
{


    public class ItemPotion : ItemConsumable
    {
        [SerializeField] PotionType type;
        [SerializeField] float strength;
        [SerializeField] float duration;


        public delegate void TakeEffect(ItemPotion potion);



        public override void OnUse(IActor actor)
        {
            DecrimentSlot();
            PlayUseAnimation(actor);
            if (OutOfItem()) OnUnequipt();
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
            effects[(int)type](this);
            ready = true;
        }


        /// <summary>
        /// This is meant to be extended with whatever potion types you want to create; by extended, I of 
        /// course mean modified to suite the game, not extention in the technical OOP sense.
        /// 
        /// The number values of the enum MUST be keyed to match the effects in effects array.  This means 
        /// the numbering must be sequential and the effect/enum labels must be in the same order as the 
        /// corresponding methods in the array below.
        /// </summary>
        [System.Serializable]
        public enum PotionType
        {
            NONE = 0,
            HEALING = 1,
            MOD_FIRE_DMG = 2
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        private static TakeEffect[] effects = new TakeEffect[]{
            NoEffect,
            HealingEffect,
            ModFireDmg

        };



        /****************************************************************************************************/
        /*                           Delegate Methods for Potion Effects                                    */
        /****************************************************************************************************/
        #region Potion Effect Methods


        private static void NoEffect(ItemPotion potion) {/*Do Nothing*/}


        private static void HealingEffect(ItemPotion potion)
        {
            EntityLiving user = potion.holder as EntityLiving;
            if (user != null) user.health.Heal(potion.strength);
        }


        private static void ModFireDmg(ItemPotion potion)
        {
            EntityLiving user = potion.holder as EntityLiving;
            if (user != null) user.AddStatusEffect(StatusEffects.EEffectType.FIRE_RESIT, potion.strength, potion.duration);
        }



        #endregion

    }


}
