//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using DistantLands.Cozy.Data;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy
{

    [ExecuteAlways]
    public class CozyWeather : CozySystem
    {

        #region Weather    
        public float cumulus;
        public float cirrus;
        public float altocumulus;
        public float cirrostratus;
        public float chemtrails;
        public float nimbus;
        public float nimbusHeightEffect;
        public float nimbusVariation;
        public float borderHeight;
        public float borderEffect;
        public float borderVariation;
        public float fogDensity;

        public float cloudCoverage => cumulus;

        #endregion

        #region Atmosphere    

        [Tooltip("Should the atmosphere be set using the physical sun height or the time of day")]
        public bool usePhysicalSunHeight;
        public float sunDirection;
        public float sunPitch;
        public Vector3 moonDirection;
        [ColorUsage(true, true)] public Color skyZenithColor;
        [ColorUsage(true, true)] public Color skyHorizonColor;
        [ColorUsage(true, true)] public Color cloudColor;
        [ColorUsage(true, true)] public Color cloudHighlightColor;
        [ColorUsage(true, true)] public Color highAltitudeCloudColor;
        [ColorUsage(true, true)] public Color sunlightColor;
        [ColorUsage(true, true)] public Color starColor;
        [ColorUsage(true, true)] public Color ambientLightHorizonColor;
        [ColorUsage(true, true)] public Color ambientLightZenithColor;
        public float ambientLightMultiplier;
        public float galaxyIntensity;
        [ColorUsage(true, true)] public Color fogColor1;
        [ColorUsage(true, true)] public Color fogColor2;
        [ColorUsage(true, true)] public Color fogColor3;
        [ColorUsage(true, true)] public Color fogColor4;
        [ColorUsage(true, true)] public Color fogColor5;
        [ColorUsage(true, true)] public Color fogFlareColor;
        [ColorUsage(true, true)] public Color fogMoonFlareColor;
        [ColorUsage(true, true)] public Color fogShadowColor;
        [ColorUsage(true, true)] public Color fogLitColor;
        public float gradientExponent = 0.364f;
        public float sunSize = 0.7f;
        [ColorUsage(true, true)] public Color sunColor;
        [ColorUsage(true, true)] public Color moonColor;
        public float sunFalloff = 43.7f;
        [ColorUsage(true, true)] public Color sunFlareColor;
        public float moonFalloff = 24.4f;
        [ColorUsage(true, true)] public Color moonlightColor;
        [ColorUsage(true, true)] public Color moonFlareColor;
        [ColorUsage(true, true)] public Color galaxy1Color;
        [ColorUsage(true, true)] public Color galaxy2Color;
        [ColorUsage(true, true)] public Color galaxy3Color;
        [ColorUsage(true, true)] public Color lightScatteringColor;
        public float lightScatteringPosition;
        public float lightScatteringHeight;
        public float constellationIntensity;
        public float rainbowPosition = 78.7f;
        public float rainbowWidth = 11;
        public Texture rainbowTexture;
        public float rainbowIntensity;
        public bool useRainbow;
        public float fogStart1 = 2;
        public float fogStart2 = 5;
        public float fogStart3 = 10;
        public float fogStart4 = 30;
        public float fogHeight = 0.85f;
        public float fogDensityMultiplier;
        public float fogLightFlareIntensity = 1;
        public float fogLightFlareFalloff = 21;
        public float fogLightFlareSquish = 1;
        public float fogSmoothness;
        public float fogVariationAmount;
        public Vector3 fogVariationDirection;
        public float fogVariationDistance;
        public float fogVariationScale;

        public float heightFogIntensity = 0f;
        public float heightFogVariationScale = 150f;
        public float heightFogVariationAmount = 150f;
        public float fogBase = 0f;
        public float heightFogTransition = 0f;
        public float heightFogDistance = 1000f;
        [ColorUsage(true, true)] public Color heightFogColor;

        [ColorUsage(true, true)] public Color cloudMoonColor;
        public float cloudSunHighlightFalloff = 14.1f;
        public float cloudMoonHighlightFalloff = 22.9f;
        public float cloudWindSpeed = 3;
        public float clippingThreshold = 0.5f;
        public float cloudMainScale = 20;
        public float cloudDetailScale = 2.3f;
        public float cloudDetailAmount = 30;
        public float acScale = 1;
        public float cirroMoveSpeed = 0.5f;
        public float cirrusMoveSpeed = 0.5f;
        public float chemtrailsMoveSpeed = 0.5f;
        [ColorUsage(true, true)] public Color cloudTextureColor = Color.white;
        public float cloudCohesion = 0.75f;
        public float spherize = 0.361f;
        public float shadowDistance = 0.0288f;
        public float cloudThickness = 2f;
        public float textureAmount = 1f;
        public Texture cloudTexture;
        public Texture chemtrailsTexture;
        public Texture cirrusCloudTexture;
        public Texture altocumulusCloudTexture;
        public Texture cirrostratusCloudTexture;
        public Texture starMap;
        public Texture starDomeTexture;
        public Texture galaxyDomeTexture;
        public Texture constellationDomeTexture;
        public Texture galaxyMap;
        public Texture galaxyStarMap;
        public Texture galaxyVariationMap;
        public Texture lightScatteringMap;
        public Vector3 texturePanDirection;
        public Texture partlyCloudyLuxuryClouds;
        public Texture mostlyCloudyLuxuryClouds;
        public Texture overcastLuxuryClouds;
        public Texture lowBorderLuxuryClouds;
        public Texture highBorderLuxuryClouds;
        public Texture lowNimbusLuxuryClouds;
        public Texture midNimbusLuxuryClouds;
        public Texture highNimbusLuxuryClouds;
        public Texture luxuryVariation;

        public float skyFogAmount = 1;
        public float cloudsFogAmount = 1;
        public float cloudsFogLightAmount = 1;
        public bool separateSunLightAndTransform;
        public float sunAngle = 0.5f;
        public LightShadows sunlightShadows = LightShadows.Soft;
        public LightShadows moonlightShadows = LightShadows.Soft;
#if COZY_URP || COZY_HDRP
        public AtmosphereProfile.SRPFlare sunFlare;
        public AtmosphereProfile.SRPFlare moonFlare;
#endif

        #endregion

        #region Filter   
        public float filterSaturation;
        public float filterValue;
        public Color filterColor;
        public Color sunFilter;
        public Color cloudFilter;

        #endregion

        #region Runtime Variables   

        private float adjustedScale;

        public enum LockToCameraStyle { useMainCamera, useCustomCamera, DontLockToCamera }

        [Tooltip("Should the weather sphere always follow the camera and automatically rescale to the scene size?")]
        public LockToCameraStyle lockToCamera;
        public static bool FreezeUpdateInEditMode = false;
        public static bool DisplayGizmos = true;
        public static bool SceneFogRendering = true;
        public static bool Tooltips
        {
            get
            {
#if UNITY_EDITOR
                return EditorPrefs.GetBool("CZY_Tooltips", true);
#else
                return false;
#endif
            }
            set
            {
#if UNITY_EDITOR
                EditorPrefs.SetBool("CZY_Tooltips", value);
#endif
            }
        }
        public static bool Graphs
        {
            get
            {
#if UNITY_EDITOR
                return EditorPrefs.GetBool("CZY_Graphs", true);
#else
                return false;
#endif
            }
            set
            {
#if UNITY_EDITOR
                EditorPrefs.SetBool("CZY_Graphs", value);
#endif
            }
        }
        public static bool FollowEditorCamera = true;
        public bool disableSunAtNight = true;
        public bool handleSceneLighting = true;
        public bool dontDestroyOnLoad;

        #endregion

        #region References


        [SerializeField]
        [Tooltip("Set the color of these particle systems to the star color of the weather system.")]
        private List<ParticleSystem> m_Stars = new List<ParticleSystem>();
        [Tooltip("Set the color of these particle systems to the cloud color of the weather system.")]
        [SerializeField]
        private List<ParticleSystem> m_CloudParticles = new List<ParticleSystem>();


        public Light sunLight;
        public Transform sunTransform;
        public bool centerAroundCustomObject;
        public Transform customPivot;
        public MeshRenderer cloudMesh;
        public MeshRenderer skyMesh;
        public MeshRenderer fogMesh;
        public Camera cozyCamera;
#if COZY_URP || COZY_HDRP
        public LensFlareComponentSRP sunLensFlare;
#endif


        #endregion

        #region Modules

        public List<Type> activeModules;

        public CozyInteractionsModule interactionsModule;
        public CozyClimateModule climateModule;
        public CozyWeatherModule weatherModule;
        public CozyTimeModule timeModule;
        public CozyAtmosphereModule atmosphereModule;
        public CozyWindModule windModule;

        public delegate void RefreshModules();
        public static event RefreshModules refreshModules;


        #endregion

        #region Events

        [System.Serializable]
        public class Events
        {

            public float timeToCheckFor;
            public int currentMinute;
            public int currentHour;

            public delegate void OnEvening();
            public static event OnEvening onEvening;
            public void RaiseOnEvening() => onEvening?.Invoke();
            public delegate void OnMorning();
            public static event OnMorning onMorning;
            public void RaiseOnMorning() => onMorning?.Invoke();
            public delegate void OnNewHour();
            public static event OnNewHour onNewHour;
            public void RaiseOnNewHour() => onNewHour?.Invoke();
            public delegate void OnMinutePass();
            public static event OnMinutePass onNewMinute;
            public void RaiseOnMinutePass() => onNewMinute?.Invoke();
            public delegate void OnNight();
            public static event OnNight onNight;
            public void RaiseOnNight() => onNight?.Invoke();
            public delegate void OnDay();
            public static event OnDay onDay;
            public void RaiseOnDay() => onDay?.Invoke();
            public delegate void OnDawn();
            public static event OnDawn onDawn;
            public void RaiseOnDawn() => onDawn?.Invoke();
            public delegate void OnAfternoon();
            public static event OnAfternoon onAfternoon;
            public void RaiseOnAfternoon() => onAfternoon?.Invoke();
            public delegate void OnTwilight();
            public static event OnTwilight onTwilight;
            public void RaiseOnTwilight() => onTwilight?.Invoke();
            public delegate void OnWeatherChange();
            public static event OnWeatherChange onWeatherChange;
            public void RaiseOnWeatherChange() => onWeatherChange?.Invoke();
            public delegate void OnDayChange();
            public static event OnDayChange onNewDay;
            public void RaiseOnDayChange() => onNewDay?.Invoke();
            public delegate void OnYearChange();
            public static event OnYearChange onNewYear;
            public void RaiseOnYearChange() => onNewYear?.Invoke();

        }

        public Events events;


        #endregion

        #region Triggers   
        [Tooltip("The tag that contains all triggers that stop weather FX from playing.")]
        public string cozyTriggerTag = "FX Block Zone";

        [HideInInspector]
        public List<Collider> cozyTriggers;
        public Dictionary<int, List<Collider>> triggersPerScene = new();

        #endregion

        #region Ecosystem  
        public List<CozySystem> systems;


        #endregion

        #region FX

        public Transform audioFXParent;
        public Transform particleFXParent;
        public Transform thunderFXParent;
        public Transform visualFXParent;

        #endregion

        #region Execution

        public delegate void FrameResetDelegate();
        public static event FrameResetDelegate OnFrameReset;
        public void RaiseOnFrameReset()
        {
            OnFrameReset?.Invoke();
        }
        public delegate void UpdateWeatherWeightsDelegate();
        public static event UpdateWeatherWeightsDelegate UpdateWeatherWeights;
        public void RaiseUpdateWeatherWeights() => UpdateWeatherWeights?.Invoke();
        public delegate void UpdateFXWeightsDelegate();
        public static event UpdateFXWeightsDelegate UpdateFXWeights;
        public void RaiseUpdateFXWeights() => UpdateFXWeights?.Invoke();
        public delegate void PropogateVariablesDelegate();
        public static event PropogateVariablesDelegate PropogateVariables;
        public void RaisePropogateVariables() => PropogateVariables?.Invoke();
        public delegate void CozyUpdateLoopDelegate();
        public static event CozyUpdateLoopDelegate CozyUpdateLoop;
        public void RaiseCozyUpdateLoop() => CozyUpdateLoop?.Invoke();

        #endregion

        public enum ControlMethod { native, profile }

        public enum SkyStyle { desktop, mobile, off }
        public SkyStyle skyStyle;

        public enum CloudStyle
        {
            cozyDesktop,
            cozyMobile,
            soft,
            paintedSkies,
            luxury,
            ghibliDesktop,
            ghibliMobile,
            singleTexture,
            off
        }
        public CloudStyle cloudStyle;

        public enum FogStyle
        {
            unity,
            stylized,
            heightFog,
            steppedFog,
            // volumetric,
            off
        }

        public FogStyle fogStyle = FogStyle.stylized;
        public CozyModule overrideWeather;

        public GameObject moduleHolder;
        public List<CozyModule> modules = new List<CozyModule>();
        public IEnumerable<CozyModule> ActiveAndEnabledModules => modules.Where(module => module.isActiveAndEnabled);

        public void SetupReferences()
        {
            if (!separateSunLightAndTransform)
            {
                sunLight = GetChild<Light>("Sun Light");
                if (sunLight == null)
                    sunLight = GetChild<Light>("Sun");

                sunTransform = sunLight.transform;
            }
            skyMesh = GetChild("Skydome").GetComponent<MeshRenderer>();
            cloudMesh = GetChild("Foreground Clouds").GetComponent<MeshRenderer>();
            fogMesh = GetChild("Fog").GetComponent<MeshRenderer>();
            audioFXParent = GetChild("Audio FX");
            particleFXParent = GetChild("Particle FX");
            visualFXParent = GetChild("Visual FX");
            thunderFXParent = GetChild("Thunder FX");

#if COZY_URP || COZY_HDRP
            if (sunFlare.flare != null)
                if (sunTransform.GetComponent<LensFlareComponentSRP>())
                    sunLensFlare = sunTransform.GetComponent<LensFlareComponentSRP>();
                else
                    sunLensFlare = sunTransform.gameObject.AddComponent<LensFlareComponentSRP>();
#endif

            if (cozyTriggers.Count == 0)
                ResetFXTriggers();

            if (handleSceneLighting)
                RenderSettings.ambientMode = AmbientMode.Trilight;

        }

        private new void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += UpdateOnSceneLoaded;
            SceneManager.sceneUnloaded += UpdateOnSceneUnLoaded;
#if UNITY_EDITOR
            SceneView.beforeSceneGui += UpdateSkydomePositionAndScale;
#endif
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= UpdateOnSceneLoaded;
            SceneManager.sceneUnloaded -= UpdateOnSceneUnLoaded;
#if UNITY_EDITOR
            SceneView.beforeSceneGui -= UpdateSkydomePositionAndScale;
#endif
        }

        private void Awake()
        {

            SetupReferences();
            ResetModules();
            ResetVariables();
            ResetQuality();
            UpdateShaderVariables();
            CozyShaderIDs.GrabShaderIDs();

            if (Application.isPlaying)
            {
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(this);
                ResetFXTriggers();
            }
        }

        public void SetupSystems()
        {
            systems = new List<CozySystem>() { this };
            systems.AddRange(FindObjectsByType<CozySystem>(FindObjectsSortMode.None).Where(x => x != this));
        }

        public void ResetFXTriggers()
        {
            cozyTriggers.Clear();
            foreach (Collider i in FindObjectsByType<Collider>(FindObjectsSortMode.None))
            {
                if (i.gameObject.tag == cozyTriggerTag)
                {
                    cozyTriggers.Add(i);
                }
            }
        }

        public void UpdateTriggersInScene(Scene scene)
        {
            var rootGameObjects = scene.GetRootGameObjects();

            if (!GetBlockZonesFromObjects(rootGameObjects, out var blockZones))
            {
                return;
            }

            triggersPerScene.Add(scene.handle, blockZones);
            RefreshTriggers();
        }

        public void RemoveTriggersInScene(Scene scene)
        {
            triggersPerScene.Remove(scene.handle);
            RefreshTriggers();
        }

        public bool GetBlockZonesFromObjects(IEnumerable<GameObject> gameObjects, out List<Collider> blockZones)
        {
            blockZones = gameObjects
                .SelectMany(item => FindChildrenComponentsByTag<Collider>(item.transform, cozyTriggerTag))
                .ToList();

            return blockZones.Count > 0;
        }

        public void RefreshTriggers()
        {
            var colliders = triggersPerScene.Values.SelectMany(trigger => trigger);
            cozyTriggers.Clear();
            cozyTriggers.AddRange(colliders);

            var cozyParticles = particleFXParent.GetComponentsInChildren<CozyParticles>();

            foreach (var cozyParticle in cozyParticles)
            {
                cozyParticle.SetupTriggers();
            }
        }

        /// <summary>
        /// Resets all of the material shaders to the shaders in the atmosphere profiles.
        /// </summary> 
        public void ResetQuality()
        {

            SetupReferences();


            switch (cloudStyle)
            {
                case CloudStyle.cozyDesktop:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Desktop Clouds Reference")).shader;
                    break;
                case CloudStyle.cozyMobile:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Mobile Clouds Reference")).shader;
                    break;
                case CloudStyle.ghibliDesktop:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Desktop Ghibli Clouds Reference")).shader;
                    break;
                case CloudStyle.ghibliMobile:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Mobile Ghibli Clouds Reference")).shader;
                    break;
                case CloudStyle.paintedSkies:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Painted Clouds Reference")).shader;
                    break;
                case CloudStyle.luxury:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Luxury Clouds Reference")).shader;
                    break;
                case CloudStyle.soft:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Soft Clouds Reference")).shader;
                    break;
                case CloudStyle.singleTexture:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Single Texture Reference")).shader;
                    break;
                case CloudStyle.off:
                    cloudMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Disabled")).shader;
                    break;
                default:
                    break;
            }

            switch (skyStyle)
            {
                case SkyStyle.desktop:
                    skyMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Desktop Sky Reference")).shader;
                    break;
                case SkyStyle.mobile:
                    skyMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Mobile Sky Reference")).shader;
                    break;
                case SkyStyle.off:
                    skyMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Disabled")).shader;
                    break;
                default:
                    break;
            }

            switch (fogStyle)
            {
                case FogStyle.stylized:
                    fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Default Fog Reference")).shader;
                    RenderSettings.fog = false;
                    break;
                case FogStyle.heightFog:
                    fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Height Fog Reference")).shader;
                    RenderSettings.fog = false;
                    break;
                // case FogStyle.volumetric:
                //     fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Volumetric Fog Reference")).shader;
                //     RenderSettings.fog = false;
                //     break;
                case FogStyle.steppedFog:
                    fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Stepped Fog Reference")).shader;
                    RenderSettings.fog = false;
                    break;
                case FogStyle.unity:
                    fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Disabled")).shader;
                    RenderSettings.fog = true;
                    break;
                case FogStyle.off:
                    fogMesh.sharedMaterial.shader = ((Material)Resources.Load("Materials/Disabled")).shader;
                    // RenderSettings.fog = false;
                    break;
                default:
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {

            RaiseOnFrameReset();
            RaiseUpdateWeatherWeights();
            RaiseUpdateFXWeights();
            RaisePropogateVariables();
            RaiseCozyUpdateLoop();

        }

        void LateUpdate()
        {
            if (Application.isPlaying)
            {
                UpdateSkydomePositionAndScale();
                return;
            }
            else if (Application.isFocused)
                UpdateSkydomePositionAndScale();
        }

        public void UpdateSkydomePositionAndScale()
        {
            if (lockToCamera != LockToCameraStyle.DontLockToCamera || centerAroundCustomObject)
            {

                if (lockToCamera == LockToCameraStyle.useMainCamera && cozyCamera == null)
                    cozyCamera = Camera.main;

                if (cozyCamera != null)
                {
                    adjustedScale = cozyCamera.farClipPlane / 1000;
                    transform.GetChild(0).localScale = Vector3.one * adjustedScale;
                }

                if (centerAroundCustomObject && customPivot)
                    transform.position = customPivot.position;
                else if (cozyCamera)
                    transform.position = cozyCamera.transform.position;
            }
        }

#if UNITY_EDITOR
        public void UpdateSkydomePositionAndScale(SceneView sceneView)
        {
            SceneFogRendering = sceneView.sceneViewState.fogEnabled;

            if (FreezeUpdateInEditMode || !FollowEditorCamera || Application.isFocused)
                return;

            if (lockToCamera != LockToCameraStyle.DontLockToCamera)
            {
                if (lockToCamera == LockToCameraStyle.useMainCamera)
                    cozyCamera = Camera.main;

                if (cozyCamera != null)
                {
                    if (Application.isPlaying)
                    {
                        transform.position = cozyCamera.transform.position;
                        adjustedScale = cozyCamera.farClipPlane / 1000;
                    }
                    else if (Camera.current != null)
                    {
                        transform.position = sceneView.camera.transform.position;
                        adjustedScale = sceneView.camera.farClipPlane / 1000;

                        if (GetModule(out CozySatelliteModule module))
                        {
                            if (module.satHolder)
                                module.satHolder.position = transform.position;
                        }
                    }

                    transform.GetChild(0).localScale = Vector3.one * adjustedScale;

                }
            }
        }
#endif

        /// <summary>
        /// Sets the actual global shader variables of the sky dome, fog dome, and cloud dome.
        /// </summary> 
        public void UpdateShaderVariables()
        {
            if (CozyShaderIDs.CZY_FogColor1ID == 0)
            {
                CozyShaderIDs.GrabShaderIDs();
            }
            if (FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (fogStyle is FogStyle.stylized or FogStyle.heightFog or FogStyle.steppedFog/** or FogStyle.volumetric*/)
            {
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogColor1ID, fogColor1);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogColor2ID, fogColor2);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogColor3ID, fogColor3);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogColor4ID, fogColor4);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogColor5ID, fogColor5);

                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogLitColorID, fogShadowColor);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogShadowColorID, fogLitColor);

                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogColorStart1ID, fogStart1);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogColorStart2ID, fogStart2);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogColorStart3ID, fogStart3);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogColorStart4ID, fogStart4);

                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogIntensityID, fogDensity);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogOffsetID, fogHeight);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_LightFlareSquishID, fogLightFlareSquish);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogSmoothnessID, fogSmoothness);
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogDepthMultiplierID, fogDensityMultiplier * fogDensity);
                else
                    Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogDepthMultiplierID, fogDensityMultiplier * fogDensity * (SceneFogRendering ? 1 : 0));
#else
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_FogDepthMultiplierID, fogDensityMultiplier * fogDensity);
#endif
                Shader.SetGlobalColor(CozyShaderIDs.CZY_LightColorID, fogFlareColor);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_FogMoonFlareColorID, fogMoonFlareColor);

                Shader.SetGlobalFloat(CozyShaderIDs.CZY_VariationAmountID, fogVariationAmount);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_VariationScaleID, fogVariationScale);
                Shader.SetGlobalVector(CozyShaderIDs.CZY_VariationWindDirectionID, fogVariationDirection);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_VariationDistanceID, fogVariationDistance);

                Shader.SetGlobalFloat(CozyShaderIDs.CZY_LightFalloffID, fogLightFlareFalloff);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_LightIntensityID, fogLightFlareIntensity);

                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogBaseID, fogBase);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogBaseVariationScaleID, heightFogVariationScale);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogBaseVariationAmountID, heightFogVariationAmount);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogTransitionID, heightFogTransition);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogDistanceID, heightFogDistance);
                Shader.SetGlobalFloat(CozyShaderIDs.CZY_HeightFogIntensityID, heightFogIntensity);
                Shader.SetGlobalColor(CozyShaderIDs.CZY_HeightFogColorID, heightFogColor);
            }

            Shader.SetGlobalColor(CozyShaderIDs.CZY_FilterColorID, filterColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_SunFilterColorID, sunFilter);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_CloudFilterColorID, cloudFilter);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_FilterSaturationID, filterSaturation);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_FilterValueID, filterValue);

            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CumulusCoverageMultiplierID, cumulus);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_NimbusMultiplierID, nimbus);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_NimbusHeightID, nimbusHeightEffect);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_NimbusVariationID, nimbusVariation);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_BorderHeightID, borderHeight);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_BorderEffectID, borderEffect);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_BorderVariationID, borderVariation);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_AltocumulusMultiplierID, altocumulus);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CirrostratusMultiplierID, cirrostratus);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_ChemtrailsMultiplierID, chemtrails);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CirrusMultiplierID, cirrus);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_CloudTextureID, cloudTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_ChemtrailsTextureID, chemtrailsTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_CirrusTextureID, cirrusCloudTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_CirrostratusTextureID, cirrostratusCloudTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_AltocumulusTextureID, altocumulusCloudTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_StarMapID, starMap);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_GalaxyStarMapID, galaxyStarMap);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_GalaxyVariationMapID, galaxyVariationMap);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_LightColumnsTextureID, lightScatteringMap);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_GalaxyMapID, galaxyMap);
            Shader.SetGlobalVector(CozyShaderIDs.CZY_TexturePanDirectionID, texturePanDirection);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_ZenithColorID, skyZenithColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_HorizonColorID, skyHorizonColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_StarColorID, starColor);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_GalaxyMultiplierID, galaxyIntensity);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_RainbowIntensityID, rainbowIntensity);

            Shader.SetGlobalTexture(CozyShaderIDs.CZY_PartlyCloudyLuxuryCloudsTextureID, partlyCloudyLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_MostlyCloudyLuxuryCloudsTextureID, mostlyCloudyLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_OvercastLuxuryCloudsTextureID, overcastLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_LowBorderLuxuryCloudsTextureID, lowBorderLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_HighBorderLuxuryCloudsTextureID, highBorderLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_LowNimbusLuxuryCloudsTextureID, lowNimbusLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_MidNimbusLuxuryCloudsTextureID, midNimbusLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_HighNimbusLuxuryCloudsTextureID, highNimbusLuxuryClouds);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_LuxuryVariationTextureID, luxuryVariation);

            Shader.SetGlobalFloat(CozyShaderIDs.CZY_PowerID, gradientExponent);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_SunSizeID, sunSize);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_SunColorID, sunColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_MoonColorID, moonColor);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_SunHaloFalloffID, sunFalloff);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_SunHaloColorID, sunFlareColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_MoonFlareColorID, moonFlareColor);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_MoonFlareFalloffID, moonFalloff);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_GalaxyColor1ID, galaxy1Color);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_GalaxyColor2ID, galaxy2Color);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_GalaxyColor3ID, galaxy3Color);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_LightColumnColorID, lightScatteringColor);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_RainbowSizeID, rainbowPosition);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_RainbowWidthID, rainbowWidth);

            if (windModule)
                Shader.SetGlobalVector(CozyShaderIDs.CZY_StormDirectionID, windModule.WindDirection);
            else
                Shader.SetGlobalVector(CozyShaderIDs.CZY_StormDirectionID, -Vector3.right);

            Shader.SetGlobalColor(CozyShaderIDs.CZY_CloudColorID, cloudColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_CloudHighlightColorID, cloudHighlightColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_AltoCloudColorID, highAltitudeCloudColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_CloudTextureColorID, cloudTextureColor);
            Shader.SetGlobalColor(CozyShaderIDs.CZY_CloudMoonColorID, cloudMoonColor);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_SunFlareFalloffID, cloudSunHighlightFalloff);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CloudMoonFalloffID, cloudMoonHighlightFalloff);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_WindSpeedID, cloudWindSpeed);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CloudCohesionID, cloudCohesion);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_SpherizeID, spherize);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_ShadowingDistanceID, shadowDistance);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_ClippingThresholdID, clippingThreshold);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CloudThicknessID, cloudThickness);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_MainCloudScaleID, cloudMainScale);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_DetailScaleID, cloudDetailScale);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_DetailAmountID, cloudDetailAmount);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_TextureAmountID, textureAmount);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_AltocumulusScaleID, acScale);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CirrostratusMoveSpeedID, cirroMoveSpeed);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CirrusMoveSpeedID, cirrusMoveSpeed);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_ChemtrailsMoveSpeedID, chemtrailsMoveSpeed);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_ConstellationIntensityID, constellationIntensity);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_LightColumnsPositionID, lightScatteringPosition);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_LightColumnsHeightID, lightScatteringHeight);

            Shader.SetGlobalTexture(CozyShaderIDs.CZY_StarDomeTextureID, starDomeTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_ConstellationDomeTextureID, constellationDomeTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_GalaxyDomeTextureID, galaxyDomeTexture);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_LightColumnsTextureID, lightScatteringMap);
            Shader.SetGlobalTexture(CozyShaderIDs.CZY_RainbowTextureID, rainbowTexture);

            Shader.SetGlobalFloat(CozyShaderIDs.CZY_SkyFogAmountID, skyFogAmount);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CloudsFogAmountID, cloudsFogAmount);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_CloudsFogLightAmountID, cloudsFogLightAmount);

            Shader.SetGlobalFloat(CozyShaderIDs.CZY_DayPercentageID, modifiedDayPercentage);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_YearPercentageID, yearPercentage);
            Shader.SetGlobalFloat(CozyShaderIDs.CZY_YearPercentageID, yearPercentage);
            Shader.SetGlobalVector(CozyShaderIDs.CZY_NorthID, North);
            Shader.SetGlobalVector(CozyShaderIDs.CZY_WestID, West);
            Shader.SetGlobalVector(CozyShaderIDs.CZY_SunDirectionParamsID, new Vector4(sunDirection, sunPitch));

            if (fogStyle == FogStyle.unity)
            {

                RenderSettings.fogColor = FilterColor(fogColor5);
                RenderSettings.fogDensity = 0.003f * fogDensity * fogDensityMultiplier;

            }


            sunTransform.parent.eulerAngles = new Vector3(0, sunDirection, sunPitch);
            sunTransform.localEulerAngles = new Vector3((modifiedDayPercentage * 360) - 90, 0, 0);
            Shader.SetGlobalVector(CozyShaderIDs.CZY_SunDirectionID, -sunTransform.forward);
#if COZY_URP || COZY_HDRP
            if (sunLensFlare)
            {
                sunLensFlare.intensity = sunFlare.flare ? (sunlightColor * sunFilter).grayscale * sunFlare.intensity : 0;
                sunLensFlare.lensFlareData = sunFlare.flare;
                sunLensFlare.allowOffScreen = sunFlare.allowOffscreen;
                sunLensFlare.radialScreenAttenuationCurve = sunFlare.screenAttenuation;
                sunLensFlare.distanceAttenuationCurve = sunFlare.screenAttenuation;
                sunLensFlare.scale = sunFlare.scale;
                sunLensFlare.occlusionRadius = sunFlare.occlusionRadius;
                sunLensFlare.useOcclusion = sunFlare.useOcclusion;
            }
#endif
            if (handleSceneLighting)
            {
                sunLight.color = sunlightColor * sunFilter;
                if (disableSunAtNight)
                    sunLight.enabled = sunLight.color.r + sunLight.color.g + sunLight.color.b > 0;
            }
            else
            {
                sunLight.enabled = false;
            }

            sunLight.shadows = sunLight.enabled ? sunlightShadows : LightShadows.None;

            if (climateModule)
            {
                if (useRainbow)
                    rainbowIntensity = climateModule.groundwaterAmount * (1 - galaxyIntensity);
                else
                    rainbowIntensity = 0;
            }

            if (handleSceneLighting)
            {
                RenderSettings.sun = sunLight;
                //RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
                RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox; // My hack
                RenderSettings.ambientMode = AmbientMode.Trilight;
                RenderSettings.ambientSkyColor = ambientLightZenithColor * ambientLightMultiplier;
                RenderSettings.ambientEquatorColor = ambientLightHorizonColor * (1 - (cumulus / 2)) * ambientLightMultiplier;
                RenderSettings.ambientGroundColor = ambientLightHorizonColor * Color.gray * (1 - (cumulus / 2)) * ambientLightMultiplier;
            }

            SetStarColors(starColor);
            SetCloudColors(cloudColor);


        }


        public T GetFXRuntimeRef<T>(string name) where T : Component
        {
            if (this == null)
                return null;

            return GetComponentsInChildren<T>().ToList().Find(x => x.transform.name == name);
        }

        /// <summary>
        /// Returns the input color filtered by the current weather filter.
        /// </summary> 
        public Color FilterColor(Color color)
        {
            float a = color.a;
            Color j;


            Color.RGBToHSV(color, out float h, out float s, out float v);

            s = Mathf.Clamp(s + filterSaturation, 0, 10);
            v = Mathf.Clamp(v + filterValue, 0, 10);
            j = Color.HSVToRGB(h, s, v);

            j *= filterColor;
            j.a = a;

            return j;

        }

        /// <summary>
        /// Statically sets variables. Only used on awake or in the editor.
        /// </summary> 
        public void ResetVariables()
        {
            rainbowIntensity = climateModule ? useRainbow ? climateModule.groundwaterAmount * (1 - starColor.a) : 0 : 0;

            ambientLightHorizonColor = FilterColor(ambientLightHorizonColor);
            ambientLightZenithColor = FilterColor(ambientLightZenithColor);

        }

        public float dayPercentage
        {
            get
            {
                float dayPercent = sunAngle;

                if (timeModule)
                {
                    dayPercent = timeModule.currentTime;
                }

                return dayPercent;
            }
        }
        public float yearPercentage
        {
            get
            {
                float yearPercentage = 0.5f;

                if (timeModule)
                {
                    yearPercentage = timeModule.yearPercentage;
                }

                return yearPercentage;
            }
        }
        public float modifiedDayPercentage
        {
            get
            {
                float dayPercent = sunAngle;

                if (timeModule)
                {
                    dayPercent = timeModule.modifiedDayPercentage;
                }

                return dayPercent;
            }
        }

        public Vector3 North => Vector3.Cross(sunTransform.parent.forward, Vector3.up);
        public Vector3 West => sunTransform.parent.forward;

        #region Modules

        /// <summary>
        /// Makes sure that all module connections are setup properly. Run this after adding or removing a module.
        /// </summary> 
        public void ResetModules()
        {

            moduleHolder = GetChild("Modules").gameObject;
            modules = moduleHolder.GetComponents<CozyModule>().ToList();

            if (!interactionsModule)
                interactionsModule = GetModule<CozyInteractionsModule>();

            if (!timeModule)
                timeModule = GetModule<CozyTimeModule>();

            if (!weatherModule)
                weatherModule = GetModule<CozyWeatherModule>();

            modules.RemoveAll(x => x == null);

            refreshModules?.Invoke();

        }

        public void InitializeModule(Type module)
        {

            if (GetModule(module))
            {
                Debug.LogWarning($"Cannot add {module.Name} because the current COZY instance already contains this module.");
                return;
            }

            CozyModule newModule = (CozyModule)moduleHolder.AddComponent(module);
            if (!newModule.CheckIfModuleCanBeAdded(out string warning))
            {
                Debug.LogWarning($"Cannot add {module.Name} due to a conflict with {warning}.");
                DestroyImmediate(newModule);
                return;
            }

            modules.Add(newModule);
            ResetModules();

        }

        public void ResetModule(CozyModule module)
        {
            StartCoroutine(ResetModuleRoutine(module));
        }

        public IEnumerator ResetModuleRoutine(CozyModule module)
        {

            Type savedType = module.GetType();

            if (!module.CheckIfModuleCanBeRemoved(out string warning))
            {
                Debug.LogWarning($"Module cannot be reset as it has dependencies on the weather sphere. Please remove the {warning} before resetting this module!");
                yield break;
            }
            modules.Remove(module);
            DestroyImmediate(module);
            ResetModules();

            yield return null;

            CozyModule newModule = (CozyModule)moduleHolder.AddComponent(savedType);

            modules.Add(newModule);
            ResetModules();

        }

        public void DeintitializeModule(CozyModule module)
        {
            if (!module.CheckIfModuleCanBeRemoved(out string warning))
            {
                Debug.LogWarning($"Module cannot be removed as it has dependencies on the weather sphere. Please remove the {warning} before removing this module!");
                return;
            }
            modules.Remove(module);
            DestroyImmediate(module);
            ResetModules();


        }

        public T GetModule<T>() where T : CozyModule
        {
            Type type = typeof(T);

            return moduleHolder.GetComponent(type) ? moduleHolder.GetComponent(type) as T : null;
        }
        public T GetModule<T>(out T module) where T : CozyModule
        {
            Type type = typeof(T);

            module = moduleHolder.GetComponent(type) ? moduleHolder.GetComponent(type) as T : null;

            return module;
        }

        public CozyModule GetModule(Type type)
        {

            return moduleHolder.GetComponent(type) ? moduleHolder.GetComponent(type) as CozyModule : null;
        }

        public void UpdateOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateTriggersInScene(scene);

            foreach (var module in ActiveAndEnabledModules)
            {
                module.OnSceneLoaded();
            }
        }

        public void UpdateOnSceneUnLoaded(Scene scene)
        {
            RemoveTriggersInScene(scene);

            foreach (var module in ActiveAndEnabledModules)
            {
                module.OnSceneUnloaded();
            }
        }
        #endregion

        #region Generic

        public Transform GetChild(string name)
        {

            foreach (Transform i in transform.GetComponentsInChildren<Transform>())
            {
                if (i.name == name)
                    return i;
            }

            return null;
        }
        public T GetChild<T>(string name) where T : Component
        {

            foreach (T i in transform.GetComponentsInChildren<T>())
            {
                if (i.name == name)
                    return i;
            }

            return default;
        }

        public List<T> FindChildrenComponentsByTag<T>(Transform parentTransform, string tag)
            where T : Component
        {
            var components = new List<T>();

            foreach (Transform child in parentTransform)
            {
                if (child && child.CompareTag(tag))
                {
                    var component = child.GetComponent<T>();

                    if (component)
                    {
                        components.Add(component);
                    }
                }

                components.AddRange(FindChildrenComponentsByTag<T>(child, tag));
            }

            return components;
        }

        /// <summary>
        /// Get the current instance of Cozy in the scene. Returns null if no weather sphere is found.
        /// </summary> 
        public static CozyWeather instance
        {

            get
            {
                if (cachedInstance)
                    return cachedInstance;

                cachedInstance = FindObjectOfType<CozyWeather>();
                return cachedInstance;


            }

        }

        private static CozyWeather cachedInstance;

        /// <summary>
        /// Sets the colors of the star particle systems.
        /// </summary> 
        private void SetStarColors(Color color)
        {

            if (m_Stars.Count == 0)
                return;

            foreach (ParticleSystem i in m_Stars)
            {

                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;

            }
        }

        /// <summary>
        /// Sets the colors of the cloud particle systems.
        /// </summary> 
        private void SetCloudColors(Color color)
        {
            if (m_CloudParticles.Count == 0)
                return;

            foreach (ParticleSystem i in m_CloudParticles)
            {
                if (i == null)
                    continue;

                ParticleSystem.MainModule j = i.main;
                j.startColor = color;

                ParticleSystem.TrailModule k = i.trails;
                k.colorOverLifetime = color;

            }
        }

        public void SetStyle(SkyStyle style)
        {
            skyStyle = style;
        }

        public void SetStyle(FogStyle style)
        {
            fogStyle = style;
        }

        public void SetStyle(CloudStyle style)
        {
            cloudStyle = style;
        }


        #endregion

    }
}