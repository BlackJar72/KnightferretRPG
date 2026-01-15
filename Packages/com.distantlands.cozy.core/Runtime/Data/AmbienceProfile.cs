//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Ambience Profile", order = 361)]
    public class AmbienceProfile : CozyProfile
    {

        [Tooltip("Specifies the minimum length for this ambience profile.")]
        [MeridiemTimeAttribute]
        public float minTime = new MeridiemTime(0, 30);
        [Tooltip("Specifies the maximum length for this ambience profile.")]
        [MeridiemTimeAttribute]
        public float maxTime = new MeridiemTime(2, 30);
        [Tooltip("Multiplier for the computational chance that this ambience profile will play; 0 being never, and 2 being twice as likely as the average.")]
        [Range(0, 2)]
        public float likelihood = 1;
        public WeightedRandomChance chance;
        [HideTitle(1)]
        public WeatherProfile[] dontPlayDuring;
        [ChanceEffector]
        public List<ChanceEffector> chances;


        [FX]
        public FXProfile[] FX;

        public float GetChance (CozyWeather weather)
        {

            float i = likelihood;

            foreach (ChanceEffector j in chances)
            {
                i *= j.GetChance(weather);
            }

            return i > 0 ? i : 0;

        }
        public void SetWeight(float weightVal)
        {
            foreach (FXProfile fx in FX)
                fx?.PlayEffect(weightVal);

        }
    }

}