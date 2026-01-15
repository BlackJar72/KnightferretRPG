//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
#if COZY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyReflectionsModule : CozyModule
    {

        public enum UpdateFrequency { everyFrame, onAwake, onHour, viaScripting }
        [CozySearchable("Reflection")]
        public UpdateFrequency updateFrequency;
        [CozySearchable]
        public Cubemap reflectionCubemap;
        public Camera reflectionCamera;
        [Tooltip("How many frames should pass before the cubemap renders again? A value of 0 renders every frame and a value of 30 renders once every 30 frames.")]
        [Range(0, 30)]
        [CozySearchable]
        public int framesBetweenRenders = 10;
        [Tooltip("What layers should be rendered into the skybox reflections?.")]
        [CozySearchable]
        public LayerMask layerMask = 2;
        public bool automaticallySetLayer;
        private int framesLeft;
        public int minimumQualityLevel;

        [Tooltip("Refresh the skybox reflections when the scene loads or unloads.")]
        [CozySearchable]
        public bool refreshOnSceneChange;
#if COZY_URP
        public int rendererOverride;
#endif

        public override void InitializeModule()
        {

            base.InitializeModule();
            reflectionCubemap = Resources.Load("Materials/Reflection Cubemap") as Cubemap;
            RenderSettings.customReflectionTexture = reflectionCubemap;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            if (automaticallySetLayer)
            {
                weatherSphere.fogMesh.gameObject.layer = ToLayer(layerMask);
                weatherSphere.skyMesh.gameObject.layer = ToLayer(layerMask);
                weatherSphere.cloudMesh.gameObject.layer = ToLayer(layerMask);
            }

            if (updateFrequency == UpdateFrequency.onAwake || updateFrequency == UpdateFrequency.onHour)
            {
                RenderReflections();
            }
            if (updateFrequency == UpdateFrequency.onHour)
            {
                CozyWeather.Events.onNewHour += RenderReflections;
            }
        }

        public override void CozyUpdateLoop()
        {
            if (weatherSphere == null)
            {
                base.InitializeModule();
            }

            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
            {
                return;
            }

            if (updateFrequency == UpdateFrequency.everyFrame)
            {
                if (framesLeft < 0)
                {

                    RenderReflections();
                    framesLeft = framesBetweenRenders;

                }
                else
                {
                    framesLeft--;
                }
            }
        }

        public override void OnSceneLoaded()
        {
            RefreshReflectionsOnSceneChange();
        }
        
        public override void OnSceneUnloaded()
        {
            RefreshReflectionsOnSceneChange();
        }

        protected void RefreshReflectionsOnSceneChange()
        {
            if (refreshOnSceneChange)
                RenderReflections();
        }
        
        public int ToLayer(LayerMask mask)
        {
            int value = mask.value;
            if (value == 0)
            {
                return 0;
            }
            for (int l = 1; l < 32; l++)
            {
                if ((value & (1 << l)) != 0)
                {
                    return l;
                }
            }
            return -1;
        }

        public override void DeinitializeModule()
        {
            base.DeinitializeModule();

            if (reflectionCamera)
            {
                DestroyImmediate(reflectionCamera.gameObject);
            }
            if (updateFrequency == UpdateFrequency.onHour)
            {
                CozyWeather.Events.onNewHour -= RenderReflections;
            }

            RenderSettings.customReflectionTexture = null;

        }

        public void RenderReflections()
        {

            if (QualitySettings.GetQualityLevel() < minimumQualityLevel || reflectionCubemap == null)
                return;

            if (!weatherSphere.cozyCamera)
            {
                Debug.LogError("COZY Reflections requires the cozy camera to be set in the settings tab!");
                return;
            }

            if (reflectionCamera == null)
            {
                SetupCamera();
            }

            reflectionCamera.enabled = true;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.nearClipPlane = weatherSphere.cozyCamera.nearClipPlane;
            reflectionCamera.farClipPlane = weatherSphere.cozyCamera.farClipPlane;
            reflectionCamera.cullingMask = layerMask;
#if COZY_URP
            if (reflectionCamera.GetComponent<UniversalAdditionalCameraData>())
                reflectionCamera.GetComponent<UniversalAdditionalCameraData>().SetRenderer(rendererOverride);
#endif
            reflectionCamera.RenderToCubemap(reflectionCubemap);
            reflectionCamera.enabled = false;

        }

        public void SetupCamera()
        {


            GameObject i = new GameObject
            {
                name = "COZY Reflection Camera",
                hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy
            };

            reflectionCamera = i.AddComponent<Camera>();
            reflectionCamera.depth = -50;
            reflectionCamera.enabled = false;

        }

    }


}