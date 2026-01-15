using UnityEngine;
using DistantLands.Cozy.Data;
using System.Collections.Generic;
using System.Collections;

namespace DistantLands.Cozy
{
    [System.Serializable]
    public class CozyForecastManagement
    {
        public ForecastProfile forecastProfile;
        public enum EcosystemStyle { manual, forecast, dailyForecast, automatic }
        [Tooltip("How should this ecosystem manage weather selection? " +
        "Manual allows you to manually select the weather profile that this ecosystem will use and the weights will adjust accordingly," +
            " Forecast allows for dynamically changing weather based on a predetermined forecast that runs entirely on it's own.")]
        public EcosystemStyle weatherSelectionMode;

        public List<WeatherPattern> currentForecast;

        [System.Serializable]
        public class WeatherPattern
        {
            public WeatherProfile profile;
            public float weatherProfileDuration;
            public float startTicks;
            public float endTicks;

        }
        public float weatherTransitionTime = 15;

        public float weatherTimer;
        [Range(0, 1)]
        public float weight;

        public WeatherProfile currentWeather;
        public WeatherProfile weatherChangeCheck;
        [WeatherRelation]
        public List<WeightedWeather> weightedWeatherProfiles;[System.Serializable]
        public class WeightedWeather
        {
            [Range(0, 1)] public float weight; public WeatherProfile profile; public bool transitioning = true;

            public IEnumerator Transition(float value, float time)
            {

                transitioning = true;
                float t = 0;
                float start = weight;

                while (t < time)
                {

                    float div = (t / time);
                    yield return new WaitForEndOfFrame();

                    weight = Mathf.Lerp(start, value, div);
                    t += Time.deltaTime;

                }

                weight = value;
                transitioning = false;

            }

        }


    }
}