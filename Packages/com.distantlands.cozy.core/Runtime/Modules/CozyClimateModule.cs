//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using DistantLands.Cozy.Data;
using UnityEngine.Serialization;
using System;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyClimateModule : CozyBiomeModuleBase<CozyClimateModule>
    {
        [CozySearchable(true, "wet", "precipitation", "hot", "cold", "humidity", "temperature")]
        public ClimateProfile climateProfile;
        public CozyWeather.ControlMethod controlMethod = CozyWeather.ControlMethod.profile;


        [CozySearchable]
        [Tooltip("Adds an offset to the local temperature. Useful for adding biomes or climate change by location or elevation")]
        public float localTemperatureFilter;
        [CozySearchable]
        [Tooltip("Adds an offset to the local precipitation. Useful for adding biomes or climate change by location or elevation")]
        public float localPrecipitationFilter;
        internal float temperatureOffset;
        internal float precipitationOffset;
        [CozySearchable]
        public float currentTemperature;
        [CozySearchable]
        public float currentPrecipitation;

        [Range(0, 1)]
        [CozySearchable]
        public float snowAmount;
        [FormerlySerializedAs("m_SnowMeltSpeed")]
        [CozySearchable]
        public float snowMeltSpeed = 0.35f;
        [Range(0, 1)]
        [CozySearchable]
        [FormerlySerializedAs("wetness")]
        public float groundwaterAmount;
        [CozySearchable]
        [FormerlySerializedAs("m_DryingSpeed")]
        public float dryingSpeed = 0.5f;
        public float snowSpeed;
        public float rainSpeed;
        
        public override void InitializeModule()
        {
            isBiomeModule = GetComponent<CozyBiome>();
            base.InitializeModule();
            
            if (isBiomeModule)
                return;
            weatherSphere.climateModule = this;
            AddBiome();

        }

        public override void CozyUpdateLoop()
        {
            ComputeBiomeWeights();

            snowAmount += Time.deltaTime * snowSpeed;

            if (snowSpeed <= 0)
                if (currentTemperature > 32)
                    snowAmount -= Time.deltaTime * snowMeltSpeed * 0.03f;

            groundwaterAmount += (Time.deltaTime * rainSpeed) + (-1 * dryingSpeed * 0.001f);

            snowAmount = Mathf.Clamp01(snowAmount);
            groundwaterAmount = Mathf.Clamp01(groundwaterAmount);

            if (controlMethod == CozyWeather.ControlMethod.profile)
            {
                if (!climateProfile)
                    return;

                currentTemperature = climateProfile.GetTemperature(weatherSphere) + localTemperatureFilter + temperatureOffset;
                currentPrecipitation = Mathf.Clamp(climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter + precipitationOffset, 0, 100);
            }

            foreach (CozyClimateModule biome in biomes)
            {
                currentTemperature = Mathf.Lerp(currentTemperature, biome.currentTemperature, biome.weight);
                currentPrecipitation = Mathf.Lerp(currentPrecipitation, biome.currentPrecipitation, biome.weight);
            }

            Shader.SetGlobalFloat("CZY_SnowAmount", snowAmount);
            Shader.SetGlobalFloat("CZY_WetnessAmount", groundwaterAmount);
        }

        public override void FrameReset()
        {

            temperatureOffset = 0;
            precipitationOffset = 0;

            snowSpeed = 0;
            rainSpeed = 0;

        }

        public float GetTemperature()
        {
            if (controlMethod == CozyWeather.ControlMethod.native)
                return currentTemperature;
            else
                return climateProfile.GetTemperature(weatherSphere) + localTemperatureFilter;

        }

        public float GetTemperature(float time)
        {

            return climateProfile.GetTemperature(weatherSphere, time) + localTemperatureFilter;

        }

        [Obsolete("Please use GetHumidity instead.")]
        public float GetPrecipitation()
        {

            return climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter;

        }
        public float GetHumidity()
        {

            return climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter;

        }

        [Obsolete("Please use GetHumidity instead.")]
        public float GetPrecipitation(float time)
        {

            return climateProfile.GetHumidity(weatherSphere, time) + localPrecipitationFilter;
        }

        public float GetHumidity(float time)
        {

            return climateProfile.GetHumidity(weatherSphere, time) + localPrecipitationFilter;
        }

        public override void DeinitializeModule()
        {
            base.DeinitializeModule();

            Shader.SetGlobalFloat("CZY_WindTime", 0);
            Shader.SetGlobalVector("CZY_WindDirection", Vector3.zero);

        }
        
        public override void UpdateBiomeModule()
        {
            if (controlMethod == CozyWeather.ControlMethod.profile)
            {
                if (!climateProfile)
                    return;

                currentTemperature = climateProfile.GetTemperature(weatherSphere) + localTemperatureFilter + temperatureOffset;
                currentPrecipitation = Mathf.Clamp(climateProfile.GetHumidity(weatherSphere) + localPrecipitationFilter + precipitationOffset, 0, 100);
            }
        }

    }

}