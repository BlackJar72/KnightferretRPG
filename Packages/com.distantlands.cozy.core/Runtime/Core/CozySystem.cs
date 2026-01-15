//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy
{
    public class CozySystem : MonoBehaviour
    {
        [Range(0, 1)]
        public float weight = 1;
        [Range(0, 1)]
        public float targetWeight = 1;

        /// <summary>
        /// Determines the order in-which systems are weighted and processed.
        /// Systems with the same priority will be weighted against each other. 
        /// Systems with a higher priority will be evaluated after ones with lower priorities, thus taking precedence.
        /// </summary>
        public int priority;
        
        public void OnEnable()
        {
            if (CozyWeather.instance)
            {
                CozyWeather.instance.SetupSystems();
            }
        }

        public void SkipTime(MeridiemTime time)
        {

        }
    }
}