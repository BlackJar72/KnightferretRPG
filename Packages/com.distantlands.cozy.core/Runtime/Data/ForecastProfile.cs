//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Forecast Profile", order = 361)]
    public class ForecastProfile : CozyProfile
    {


        [Tooltip("The weather profiles that this profile will forecast.")]
        public List<WeatherProfile> profilesToForecast;

        [Tooltip("The weather profile that this profile will forecast initially.")]
        public WeatherProfile initialProfile;
        [Tooltip("The weather profiles that this profile will forecast initially.")]
        public List<CozyEcosystem.WeatherPattern> initialForecast;

        public enum StartWeatherWith { Random, InitialProfile, InitialForecast }
        public StartWeatherWith startWeatherWith;

        [Tooltip("The amount of weather profiles to forecast ahead.")]
        public int forecastLength;

    }

}

