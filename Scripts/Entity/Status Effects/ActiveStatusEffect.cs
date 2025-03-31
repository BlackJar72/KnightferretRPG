using UnityEngine;



namespace kfutils.rpg {

    public class ActiveStatusEffect {

        private IStatusEffect effect;
        private EntityLiving effected;
        private float endTime; // When it should end; FIXME: The datatype may need to change


        /// <summary>
        /// Applies the effect to the entity it is attached to.
        /// 
        /// This will return true if the effect's duration is up 
        /// (i.e., if the effect should end).
        /// </summary>
        /// <returns>If the duration is up and the effect should end.</returns>
        public bool ApplyForFrame() {
            bool over = Time.time > endTime;
            if(over) effect.EndEffect(effected); 
            else effect.EffectEntityForFrame(effected);
            return over;
        }

    }

}
