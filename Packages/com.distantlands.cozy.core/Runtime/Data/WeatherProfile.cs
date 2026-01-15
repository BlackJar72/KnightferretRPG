//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using UnityEngine.Serialization;




namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Weather Profile", order = 361)]
    public class WeatherProfile : ScriptableObject
    {

        [Tooltip("Specifies the minimum length for this weather profile in in-game hours and minutes.")]
        [FormerlySerializedAs("minWeatherTime")]
        [MeridiemTime]
        public float minTime = 0.25f;
        public float minWeatherTime => minTime;
        [Tooltip("Specifies the maximum length for this weather profile in in-game hours and minutes.")]
        [FormerlySerializedAs("maxWeatherTime")]
        [MeridiemTime]
        public float maxTime = 0.35f;
        public float maxWeatherTime => maxTime;
        public WeightedRandomChance chance;
        [HideTitle]
        [Tooltip("Allow only these weather profiles to immediately follow this weather profile in a forecast.")]
        public WeatherProfile[] forecastNext;
        public enum ForecastModifierMethod { forecastNext, DontForecastNext, forecastAnyProfileNext }
        public ForecastModifierMethod forecastModifierMethod = ForecastModifierMethod.forecastAnyProfileNext;

        [FX]
        public FXProfile[] FX;

        public float GetChance(CozyWeather weather, float inTime) => chance.GetChance(weather, inTime);
        public float GetChance(CozyWeather weather) => chance.GetChance(weather);
        public void SetWeatherWeight(float weightVal)
        {
            foreach (FXProfile fx in FX)
                fx?.PlayEffect(weightVal);
        }

    }

}