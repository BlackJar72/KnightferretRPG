//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System;
using UnityEngine;
#if THE_VISUAL_ENGINE
using TheVisualEngine;
#elif THE_VEGETATION_ENGINE
using TheVegetationEngine;
#endif

namespace DistantLands.Cozy
{

    [ExecuteAlways]
    public class CozyPureNatureModule : CozyModule
    {

        public enum UpdateFrequency { everyFrame, onAwake, viaScripting }
        public UpdateFrequency updateFrequency;

        private CozyWindModule wind;


        [Tooltip("Base wind animate the trunks")]
        [Range(0f, 5f)]
        public float baseWindPower = 3f;
        [Tooltip("Base wind animate the trunks")]
        public float baseWindSpeed = 1f;

        [Tooltip("Bursts are managed by a moving World-Space noise that multiply the base wind speed and power")]
        [Range(0f, 10f)]
        public float burstsPower = 0.5f;
        [Tooltip("Speed of the Bursts noise")]
        public float burstsSpeed = 5f;
        [Tooltip("Size of the Bursts noise in Word-Space")]
        public float burstsScale = 10f;

        [Tooltip("Micro wind animate the leaves")]
        [Range(0f, 1f)]
        public float microPower = 0.1f;
        [Tooltip("Micro wind animate the leaves")]
        public float microSpeed = 1f;
        [Tooltip("Micro wind animate the leaves")]
        public float microFrequency = 3f;

        public float renderDistance = 30f;

        public override void InitializeModule()
        {
            if (!enabled)
                return;

            SetupModule(new Type[1] { typeof(CozyWindModule) });
            wind = weatherSphere.GetModule<CozyWindModule>();
            base.InitializeModule();

            if (updateFrequency != UpdateFrequency.viaScripting)
                UpdateIntegration();

        }


        // Update is called once per frame
        public override void CozyUpdateLoop()
        {
            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (updateFrequency == UpdateFrequency.everyFrame)
                UpdateIntegration();
        }

        public void UpdateIntegration()
        {

            UpdateWind();

        }

        void UpdateWind()
        {
            Shader.SetGlobalFloat("WindPower", baseWindPower * wind.windAmount);
            Shader.SetGlobalFloat("WindSpeed", baseWindSpeed * wind.windSpeed);
            Shader.SetGlobalFloat("WindBurstsPower", burstsPower * wind.windGusting);
            Shader.SetGlobalFloat("WindBurstsSpeed", burstsSpeed * wind.windSpeed);
            Shader.SetGlobalFloat("WindBurstsScale", burstsScale);
            Shader.SetGlobalFloat("MicroPower", microPower * wind.windGusting);
            Shader.SetGlobalFloat("MicroSpeed", microSpeed * wind.windSpeed);
            Shader.SetGlobalFloat("MicroFrequency", microFrequency);
            Shader.SetGlobalFloat("GrassRenderDist", renderDistance);
        }

    }
}