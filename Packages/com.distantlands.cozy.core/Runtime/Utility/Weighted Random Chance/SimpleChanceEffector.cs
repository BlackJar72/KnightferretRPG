using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{
    
    [Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/WRC/Simple Chance", order = 361)]
    public class SimpleChanceEffector : CustomCozyChanceEffector
    {
        public float chance;
        public override float GetChance()
        {
            return chance;
        }
    }


#if UNITY_EDITOR
    

#endif

}