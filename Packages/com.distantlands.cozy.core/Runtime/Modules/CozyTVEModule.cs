using UnityEngine;
#if THE_VISUAL_ENGINE
using TheVisualEngine;
#elif THE_VEGETATION_ENGINE
using TheVegetationEngine;
#endif

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyTVEModule : CozyModule
    {
        public enum UpdateFrequency { everyFrame, onAwake, viaScripting }
        public UpdateFrequency updateFrequency;

        [Header("Control Settings")]
        [Tooltip("Enable motion integration with TVE")]
        public bool enableMotionControl = true;
        [Tooltip("Enable season integration with TVE")]
        public bool enableSeasonControl = true;
        [Tooltip("Enable wetness integration with TVE")]
        public bool enableWetnessControl = true;
        [Tooltip("Enable snow integration with TVE")]
        public bool enableSnowControl = true;

#if THE_VISUAL_ENGINE
        public TVEManager visualManager;
#elif THE_VEGETATION_ENGINE
        public TVEGlobalControl globalControl;
        public TVEGlobalMotion globalMotion;
#endif

        void Awake()
        {
            InitializeModule();

#if THE_VEGETATION_ENGINE || THE_VISUAL_ENGINE
            if (updateFrequency == UpdateFrequency.onAwake)
                UpdateTVE();
#endif
        }

        public override void InitializeModule()
        {
            if (!enabled)
                return;

            base.InitializeModule();

            if (!weatherSphere)
            {
                enabled = false;
                return;
            }

#if THE_VISUAL_ENGINE
            if (!visualManager)
                visualManager = FindObjectOfType<TVEManager>();

            if (!visualManager)
            {
                enabled = false;
                return;
            }

            visualManager.mainLight = weatherSphere.sunLight;
#elif THE_VEGETATION_ENGINE
            if (!globalControl)
                globalControl = FindObjectOfType<TVEGlobalControl>();

            if (!globalControl)
            {
                enabled = false;
                return;
            }

            if (!globalMotion)
                globalMotion = FindObjectOfType<TVEGlobalMotion>();

            if (!globalMotion)
            {
                enabled = false;
                return;
            }

            globalControl.mainLight = weatherSphere.sunLight;
#endif
        }

        void Update()
        {
            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (updateFrequency == UpdateFrequency.everyFrame)
                UpdateTVE();
        }

        public void UpdateTVE()
        {
#if THE_VEGETATION_ENGINE
            if (weatherSphere.climateModule)
            {
                if (enableWetnessControl)
                    globalControl.globalWetness = weatherSphere.climateModule.groundwaterAmount;
                
                if (enableSnowControl)
                    globalControl.globalOverlay = weatherSphere.climateModule.snowAmount;
            }

            if (enableSeasonControl)
                globalControl.seasonControl = Mathf.Clamp(weatherSphere.timeModule.yearPercentage * 4, 0, 4);

            if (enableMotionControl)
            {
                float windPower = 0f;
                Vector3 windDirection = Vector3.forward;

                if (weatherSphere.windModule != null)
                {
                    // Scale wind power from Cozy's 0-2 range to TVE's 0-1 range by halving it
                    windPower = weatherSphere.windModule.windAmount * 0.5f;
                    windDirection = weatherSphere.windModule.WindDirection;

                    // Safety check for NaN or infinity
                    if (float.IsNaN(windPower) || float.IsInfinity(windPower))
                    {
                        windPower = 0f;
                    }
                }

                // Clamp to 1 just in case wind goes beyond TVE's maximum
                globalMotion.windPower = Mathf.Clamp01(windPower);
                globalMotion.transform.LookAt(globalMotion.transform.position + windDirection, Vector3.up);
            }
#elif THE_VISUAL_ENGINE
            if (weatherSphere.climateModule)
            {
                if (enableWetnessControl)
                    visualManager.globalAtmoData.wetnessIntensity = weatherSphere.climateModule.groundwaterAmount;
                
                if (enableSnowControl)
                    visualManager.globalAtmoData.overlayIntensity = weatherSphere.climateModule.snowAmount;
            }
            
            if (enableSeasonControl)
                visualManager.seasonControl = Mathf.Clamp(weatherSphere.timeModule.yearPercentage * 4, 0, 4);

            if (enableMotionControl)
            {
                float windPower = 0f;
                Vector3 windDirection = Vector3.forward;

                if (weatherSphere.windModule != null)
                {
                    // Scale wind power from Cozy's 0-2 range to TVE's 0-1 range by halving it
                    windPower = weatherSphere.windModule.windAmount * 0.5f;
                    windDirection = weatherSphere.windModule.WindDirection;

                    // Safety check for NaN or infinity
                    if (float.IsNaN(windPower) || float.IsInfinity(windPower))
                    {
                        windPower = 0f;
                    }
                }

                // Clamp to 1 just in case wind goes beyond TVE's maximum
                visualManager.motionControl = Mathf.Clamp01(windPower);
                visualManager.transform.LookAt(visualManager.transform.position + windDirection, Vector3.up);
            }
#endif
        }
    }
}