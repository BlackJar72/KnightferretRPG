using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    /// <summary>
    /// Keep track of status effects here.  TODO: Create status effect system.
    /// </summary>
    public static class StatusEffectManagement {

        private static List<ActiveStatusEffect> transientEffects = new ();


        public static void Initialize() {
            transientEffects.Clear();
        }


        public static void UpdateTransientEffects() {
            for(int i = transientEffects.Count - 1; i > -1; i--) {
                if(transientEffects[i].ApplyForFrame()) transientEffects.RemoveAt(i);
            }
        }


        public static void AddTransientEffect(ActiveStatusEffect effect) {
            transientEffects.Add(effect);
        }


    }

}