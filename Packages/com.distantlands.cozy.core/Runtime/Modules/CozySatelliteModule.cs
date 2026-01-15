//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEngine;
using DistantLands.Cozy.Data;
#if COZY_URP || COZY_HDRP
using UnityEngine.Rendering;
#endif

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozySatelliteModule : CozyModule
    {


        [CozySearchable("moon", "satellite")]
        public SatelliteProfile[] satellites = new SatelliteProfile[0];
        [HideInInspector]
        public Transform satHolder = null;
        [CozySearchable]
        public bool hideInHierarchy = true;
        private Light moonLight;
        [CozySearchable]
        public int mainMoon;
        [CozySearchable]
        public bool useLight = true;
#if COZY_URP || COZY_HDRP
        public LensFlareComponentSRP moonLensFlare;
#endif

        public enum MoonPhase
        {
            newMoon, waxingCrescent, firstQuarter, waxingGibbous, fullMoon, waningGibbous, thirdQuarter, waningCrescent
        }


        public override void InitializeModule()
        {
            moonLight = weatherSphere.GetChild<Light>("Moon Light");
            moonLight.enabled = true;
            base.InitializeModule();
#if COZY_URP || COZY_HDRP
            if (weatherSphere.moonFlare.flare != null)
                if (moonLight.GetComponent<LensFlareComponentSRP>())
                    moonLensFlare = moonLight.GetComponent<LensFlareComponentSRP>();
                else
                    moonLensFlare = moonLight.gameObject.AddComponent<LensFlareComponentSRP>();
#endif
        }

        // Start is called before the first frame update
        void Awake()
        {
            UpdateSatellites();

        }

        // Update is called once per frame
        void Update()
        {
            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            if (satHolder == null)
            {
                UpdateSatellites();
            }
            if (moonLight == null)
            {
                moonLight = weatherSphere.GetChild<Light>("Moon Light");
            }

            if (satHolder.hideFlags == (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild) && hideInHierarchy)
                UpdateSatellites();

            if (weatherSphere.cozyCamera && Application.isPlaying)
                satHolder.position = weatherSphere.cozyCamera.transform.position;

            if (satellites != null)
                foreach (SatelliteProfile sat in satellites)
                {
                    if (!sat)
                        break;

                    if (sat.orbitRef == null)
                        UpdateSatellites();

                    if (sat.changedLastFrame == true)
                        UpdateSatellites();


                    if (sat.linkToDay && weatherSphere.timeModule)
                    {
                        float dec = sat.declination * Mathf.Sin(Mathf.PI * 2 * (((weatherSphere.modifiedDayPercentage - 0.5f) + (float)(weatherSphere.timeModule.currentDay + sat.rotationPeriodOffset + weatherSphere.timeModule.DaysPerYear * weatherSphere.timeModule.currentYear) % sat.declinationPeriod) / sat.declinationPeriod));
                        sat.orbitRef.localEulerAngles = new Vector3(0, weatherSphere.sunDirection + sat.satelliteDirection, weatherSphere.sunPitch + sat.satellitePitch + dec);
                        sat.satelliteRotation = (weatherSphere.modifiedDayPercentage - 0.5f + (float)(sat.rotationPeriodOffset + weatherSphere.timeModule.AbsoluteDay) % sat.rotationPeriod) / sat.rotationPeriod * 360;
                        sat.orbitRef.GetChild(0).localEulerAngles = Vector3.right * ((360 * weatherSphere.modifiedDayPercentage) + sat.satelliteRotation + sat.orbitOffset - 90);
                    }
                    else
                    {
                        sat.orbitRef.localEulerAngles = new Vector3(0, weatherSphere.sunDirection + sat.satelliteDirection, weatherSphere.sunPitch + sat.satellitePitch);
                        sat.orbitRef.GetChild(0).localEulerAngles = Vector3.right * ((360 * weatherSphere.dayPercentage) + sat.orbitOffset);
                        sat.satelliteRotation += Time.deltaTime * sat.satelliteRotateSpeed;
                        sat.moonRef.localEulerAngles = sat.initialRotation + sat.satelliteRotateAxis.normalized * sat.satelliteRotation;
                    }
                }

            if (!moonLight)
                return;

            if (satellites.Length == 0)
            {
                weatherSphere.moonDirection = Vector3.up;
                moonLight.transform.forward = Vector3.up;
                Shader.SetGlobalVector("CZY_MoonDirection", weatherSphere.moonDirection);
                return;
            }


            weatherSphere.moonDirection = -moonLight.transform.forward;
            Shader.SetGlobalVector("CZY_MoonDirection", -moonLight.transform.forward);

            if (mainMoon >= satellites.Length)
                mainMoon = satellites.Length - 1;

            float moonBrightness = Mathf.Clamp01(Mathf.Sin((weatherSphere.dayPercentage + 0.25f) * 2 * Mathf.PI) + 0.25f) * Mathf.Clamp01(4 * Vector3.Dot(moonLight.transform.forward, Vector3.down));

            moonLight.transform.forward = satellites[mainMoon].orbitRef.GetChild(0).forward;
            moonLight.enabled = weatherSphere.moonlightColor.grayscale > 0.05f && satellites.Length > 0 && useLight && !weatherSphere.sunLight.enabled;
            moonLight.color = weatherSphere.moonlightColor * weatherSphere.sunFilter * moonBrightness;
            moonLight.shadows = moonLight.enabled ? weatherSphere.moonlightShadows : LightShadows.None;

#if COZY_URP || COZY_HDRP
            if (moonLensFlare)
            {
                moonLensFlare.intensity = weatherSphere.moonFlare.flare ? moonBrightness : 0;
                moonLensFlare.lensFlareData = weatherSphere.moonFlare.flare;
                moonLensFlare.allowOffScreen = weatherSphere.moonFlare.allowOffscreen;
                moonLensFlare.radialScreenAttenuationCurve = weatherSphere.moonFlare.screenAttenuation;
                moonLensFlare.distanceAttenuationCurve = weatherSphere.moonFlare.screenAttenuation;
                moonLensFlare.scale = weatherSphere.moonFlare.scale;
                moonLensFlare.occlusionRadius = weatherSphere.moonFlare.occlusionRadius;
                moonLensFlare.useOcclusion = weatherSphere.moonFlare.useOcclusion;
            }
#endif


        }


        public void UpdateSatellites()
        {

            Transform oldHolder = null;


            if (satHolder)
            {
                oldHolder = satHolder;
            }

            satHolder = new GameObject("Cozy Satellites").transform;
            if (hideInHierarchy)
                satHolder.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
            else
                satHolder.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;



            if (satellites != null)
                foreach (SatelliteProfile i in satellites)
                {
                    InitializeSatellite(i);
                }

            if (oldHolder)
                DestroyImmediate(oldHolder.gameObject);

        }

        public void DestroySatellites()
        {

            if (satHolder)
                DestroyImmediate(satHolder.gameObject);

        }

        public void DestroySatellite(SatelliteProfile sat)
        {

            if (sat.orbitRef)
                DestroyImmediate(sat.orbitRef.gameObject);

        }

        public override void DeinitializeModule()
        {
            moonLight.enabled = false;
            DestroySatellites();
            Shader.SetGlobalVector("CZY_MoonDirection", Vector3.down);
        }

        public void InitializeSatellite(SatelliteProfile sat)
        {


            float dist = 0;

            if (weatherSphere.lockToCamera != CozyWeather.LockToCameraStyle.DontLockToCamera && weatherSphere.cozyCamera)
                dist = .92f * weatherSphere.cozyCamera.farClipPlane * sat.distance;
            else
                dist = .92f * 1000 * sat.distance * weatherSphere.transform.localScale.x;

            sat.orbitRef = new GameObject(sat.name).transform;
            sat.orbitRef.parent = satHolder;
            sat.orbitRef.transform.localPosition = Vector3.zero;
            var orbitArm = new GameObject("Orbit Arm");
            orbitArm.transform.parent = sat.orbitRef;
            orbitArm.transform.localPosition = Vector3.zero;
            orbitArm.transform.localEulerAngles = Vector3.zero;
            sat.moonRef = Instantiate(sat.satelliteReference, Vector3.forward * dist, Quaternion.identity, sat.orbitRef.GetChild(0)).transform;
            sat.moonRef.transform.localPosition = -Vector3.forward * dist;
            sat.moonRef.transform.localEulerAngles = sat.initialRotation;
            sat.moonRef.transform.localScale = sat.satelliteReference.transform.localScale * sat.size * (sat.autoScaleByDistance ? dist / 1000 : 1);
            sat.orbitRef.localEulerAngles = new Vector3(0, sat.satelliteDirection, sat.satellitePitch);
            sat.orbitRef.GetChild(0).localEulerAngles = Vector3.right * ((360 * weatherSphere.dayPercentage) + sat.orbitOffset);
            sat.changedLastFrame = false;
        }

        void Reset()
        {
            List<SatelliteProfile> profiles = new List<SatelliteProfile>
            {
                Resources.Load("Profiles/Satellites/Stylized Moon") as SatelliteProfile
            };
            satellites = profiles.ToArray();
        }

        public MoonPhase GetMoonPhase()
        {
            if (!weatherSphere.timeModule || satellites.Length == 0)
                return MoonPhase.newMoon;

            SatelliteProfile moon = satellites[mainMoon];


            int phase = Mathf.FloorToInt(
                ((weatherSphere.timeModule.AbsoluteDay + moon.rotationPeriodOffset + 1) % moon.rotationPeriod) / (moon.rotationPeriod / 8f));

            return (MoonPhase)Mathf.Clamp(phase, 0, 7);

        }

        public string GetMoonPhaseName()
        {
            string name = "New Moon";
            switch (GetMoonPhase())
            {
                case MoonPhase.newMoon:
                    name = "New Moon";
                    break;
                case MoonPhase.waxingCrescent:
                    name = "Waxing Crescent";
                    break;
                case MoonPhase.firstQuarter:
                    name = "First Quarter";
                    break;
                case MoonPhase.waxingGibbous:
                    name = "Waxing Gibbous";
                    break;
                case MoonPhase.fullMoon:
                    name = "Full Moon";
                    break;
                case MoonPhase.waningGibbous:
                    name = "Waning Gibbous";
                    break;
                case MoonPhase.thirdQuarter:
                    name = "Third Quarter";
                    break;
                case MoonPhase.waningCrescent:
                    name = "Waning Crescent";
                    break;
            }

            return name;
        }
    }

}