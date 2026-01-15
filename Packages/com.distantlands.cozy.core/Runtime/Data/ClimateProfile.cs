//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;



namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Climate Profile", order = 361)]
    public class ClimateProfile : CozyProfile
    {


        [Tooltip("The global temperature during the year. the x-axis is the current day over the days in the year and the y axis is the temperature in Fahrenheit.")]
        public AnimationCurve temperatureOverYear;
        [Tooltip("The global humidity during the year. the x-axis is the current day over the days in the year and the y axis is the humidity.")]
        public AnimationCurve humidityOverYear;
        [Tooltip("The local temperature during the day. the x-axis is the current ticks over 360 and the y axis is the temperature change in Fahrenheit from the global temperature.")]
        public AnimationCurve temperatureOverDay;
        [Tooltip("The local humidity during the day. the x-axis is the current ticks over 360 and the y axis is the humidity change from the global precipitation.")]
        public AnimationCurve humidityOverDay;

        [Tooltip("Adds an offset to the global temperature. Useful for adding biomes or climate change by location or elevation")]
        public float temperatureFilter;
        [Tooltip("Adds an offset to the global precipitation. Useful for adding biomes or climate change by location or elevation")]
        public float humidityFilter;

        public float GetTemperature()
        {

            CozyWeather weather = CozyWeather.instance;

            float i = (temperatureOverYear.Evaluate(weather.yearPercentage) * temperatureOverDay.Evaluate(weather.modifiedDayPercentage)) + temperatureFilter;

            return i;
        }
        public float GetTemperature(CozyWeather weather)
        {
            if (weather == null)
                return GetTemperature();

            float i = (temperatureOverYear.Evaluate(weather.yearPercentage) * temperatureOverDay.Evaluate(weather.modifiedDayPercentage)) + temperatureFilter;


            return i;
        }
        public float GetTemperature(CozyWeather weather, float time)
        {

            if (!weather.timeModule)
                return GetTemperature(weather);

            float i = (temperatureOverYear.Evaluate(time / weather.timeModule.DaysPerYear) * temperatureOverDay.Evaluate(time % 1)) + temperatureFilter;

            return i;
        }
        public float GetHumidity()
        {
            CozyWeather weather = CozyWeather.instance;
            float i = (humidityOverYear.Evaluate(weather.yearPercentage) * humidityOverDay.Evaluate(weather.modifiedDayPercentage)) + humidityFilter;

            return i;
        }
        public float GetHumidity(CozyWeather weather)
        {
            if (weather == null)
                weather = CozyWeather.instance;
                
            float i = (humidityOverYear.Evaluate(weather.yearPercentage) * humidityOverDay.Evaluate(weather.modifiedDayPercentage)) + humidityFilter;

            return i;
        }
        public float GetHumidity(CozyWeather weather, float time)
        {
            if (!weather.timeModule)
                return GetHumidity(weather);

            float i = (humidityOverYear.Evaluate(time / weather.timeModule.DaysPerYear) * humidityOverDay.Evaluate(time % 1)) + humidityFilter;

            return i;
        }

    }


}