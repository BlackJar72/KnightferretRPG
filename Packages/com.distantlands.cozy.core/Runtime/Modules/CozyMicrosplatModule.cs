//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula


using UnityEngine;

namespace DistantLands.Cozy
{

    [ExecuteAlways]
    public class CozyMicrosplatModule : CozyModule
    {

        public enum UpdateFrequency { everyFrame, onAwake, viaScripting }
        [CozySearchable("Microsplat")]
        public UpdateFrequency updateFrequency;

        [Header("Wetness")]
        [CozySearchable]
        public bool updateWetness = true;
        [Range(0f, 1f)]
        [CozySearchable]
        public float minWetness = 0f;
        [Range(0f, 1f)]
        [CozySearchable]
        public float maxWetness = 1f;
        [Header("Rain Ripples")]
        [CozySearchable]
        public bool updateRainRipples = true;
        [Header("Puddle Settings")]
        [CozySearchable]
        public bool updatePuddles = true;
        [Header("Stream Settings")]
        [CozySearchable]
        public bool updateStreams = true;
        [Header("Snow Settings")]
        [CozySearchable]
        public bool updateSnow = true;
        [Header("Wind Settings")]
        [CozySearchable]
        public bool updateWindStrength = true;

        private static readonly int GlobalSnowLevel = Shader.PropertyToID("_Global_SnowLevel");
        private static readonly int GlobalWetnessParams = Shader.PropertyToID("_Global_WetnessParams");
        private static readonly int GlobalPuddleParams = Shader.PropertyToID("_Global_PuddleParams");
        private static readonly int GlobalRainIntensity = Shader.PropertyToID("_Global_RainIntensity");
        private static readonly int GlobalStreamMax = Shader.PropertyToID("_Global_StreamMax");
        private static readonly int GlobalWindParticulateStrength = Shader.PropertyToID("_Global_WindParticulateStrength");
        private static readonly int GlobalSnowParticulateStrength = Shader.PropertyToID("_Global_SnowParticulateStrength");


        // Start is called before the first frame update
        public override void InitializeModule()
        {
            base.InitializeModule();
            
            if (updateFrequency == UpdateFrequency.onAwake)
            {
                UpdateShaderProperties();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            
            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;
                
            if (updateFrequency == UpdateFrequency.everyFrame)
            {
                UpdateShaderProperties();
            }
        }

        public void UpdateShaderProperties()
        {

            if (weatherSphere.climateModule)
            {
                if (updateSnow)
                {
                    Shader.SetGlobalFloat(GlobalSnowLevel, weatherSphere.climateModule.snowAmount);
                }
                if (updateWetness)
                {
                    float currentWetness = Mathf.Clamp(weatherSphere.climateModule.groundwaterAmount, minWetness, maxWetness);
                    Shader.SetGlobalVector(GlobalWetnessParams, new Vector2(minWetness, currentWetness));
                }
                if (updatePuddles)
                {
                    Shader.SetGlobalFloat(GlobalPuddleParams, weatherSphere.climateModule.groundwaterAmount);
                }
                if (updateRainRipples)
                {
                    Shader.SetGlobalFloat(GlobalRainIntensity, weatherSphere.climateModule.groundwaterAmount);
                }
                if (updateStreams)
                {
                    Shader.SetGlobalFloat(GlobalStreamMax, weatherSphere.climateModule.groundwaterAmount);
                }
            }

            // if (weatherSphere.vfxModule)
            // {
            //     if (updateWindStrength)
            //     {
            //         Shader.SetGlobalFloat(GlobalWindParticulateStrength, weatherSphere.vfxModule.windManager.windSpeed);
            //     }
            //     if (updateSnow && updateWindStrength)
            //     {
            //         Shader.SetGlobalFloat(GlobalSnowParticulateStrength, weatherSphere.vfxModule.windManager.windSpeed);
            //     }
            // }
        }
    }

}