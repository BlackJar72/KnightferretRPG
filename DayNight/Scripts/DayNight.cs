using System;
using kfutils;
using kfutils.rpg;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace kfutils.skies
{

    public class DayNight : MonoBehaviour
    {
        const float twoPI = Mathf.PI * 2.0f;
        const float halfPI = Mathf.PI * 0.5f;

        [SerializeField] WorldTime worldTime;
        [SerializeField] GameObject sun;
        [SerializeField] Light sunLight;
        [SerializeField] GameObject moon;
        [SerializeField] Light moonLight;
        [SerializeField] MoonPhase[] moons = new MoonPhase[10];
        [SerializeField] int curMoonPhase = 0;

        [SerializeField] Gradient sunColor;
        [SerializeField] Gradient moonColor;

        [SerializeField] Gradient skyLowColor;
        [SerializeField] Gradient skyHighColor;

        [SerializeField] Gradient cloudLightColor;
        [SerializeField] Gradient cloudDarkColor;

        [SerializeField] Gradient ambientColor;

        [SerializeField] Material skyMaterial;
        [SerializeField] VolumeProfile mainPostVolume;

        [SerializeField] float realAngle;

        private bool moonChecked = false;


        private void Start()
        {
            skyMaterial.SetTexture("_Moon", moons[curMoonPhase].MoonSky);        
        }


        private void Update()
        {
            // WTF?! Setting the angle here results in the real angle being 180 - (60 * angle)?!?!
            sun.transform.rotation = quaternion.EulerXYZ(90, 0, WorldTime.TimeInDay * twoPI - Mathf.PI);
            moon.transform.rotation = quaternion.EulerXYZ(90, 0, WorldTime.TimeInDay * twoPI - Mathf.PI);
            realAngle = sun.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            float dayness = (1.0f - Mathf.Cos(realAngle)) * 0.5f;
            SetAmbient(ambientColor.Evaluate(dayness));
            sunLight.color = sunColor.Evaluate(dayness);
            moonLight.color = moonColor.Evaluate(dayness);
            skyMaterial.SetColor("_Low_Color", skyLowColor.Evaluate(dayness));
            skyMaterial.SetColor("_High_Color", skyHighColor.Evaluate(dayness));
            skyMaterial.SetColor("_Cloud_Light", cloudLightColor.Evaluate(dayness));
            skyMaterial.SetColor("_Cloud_Dark", cloudDarkColor.Evaluate(dayness));
            sunLight.intensity = Mathf.Sqrt(Mathf.Max(0.2f - Mathf.Cos(realAngle), 0));
            RenderSettings.ambientIntensity = sunLight.intensity;
            sun.SetActive(sunLight.intensity > 0);
            skyMaterial.SetFloat("_Solar_Angle", realAngle + halfPI);
            skyMaterial.SetColor("_Sun_Color", sunColor.Evaluate(dayness));
            skyMaterial.SetFloat("_Sun_Size", 100.0f * dayness);
            UpdateMoon(dayness);
            UpdatePostProcessing(dayness);
            #if UNITY_EDITOR
            //Tests();
            #endif
        }


        private void UpdateMoon(float brightness)
        {
            MoonPhase phase = moons[curMoonPhase];
            if(!moonChecked && (brightness > 0.8f)) 
            {
                moonChecked = true;
                if(phase.NextStart == WorldTime.DayOfMonth)
                {
                    curMoonPhase = phase.NextPhaseIndex;
                    phase = moons[curMoonPhase];
                    skyMaterial.SetTexture("_Moon", phase.MoonSky);
                }    
            }
            if(moonChecked && sun.activeSelf)
            {
                moonChecked = false;
            }
            moonLight.intensity = Mathf.Sqrt(Mathf.Max(0.2f - Mathf.Cos(realAngle + Mathf.PI), 0)) 
                                    * 0.5f * phase.Brightness;        
        }


        private void UpdatePostProcessing(float brightness)
        {
            if(mainPostVolume == null) return;
            float darkness = Mathf.Clamp01(1.5f - (brightness * 3.0f));
            //Debug.Log("brightness " + brightness + " => darkness " + darkness);
            // Bloom
            // mainPostVolume.TryGet(out Bloom bloom);
            // bloom.intensity.value = darkness;
            // bloom.threshold.value = 0.9f - (0.65f * darkness);
            // bloom.scatter.value = 0.7f + (0.3f * darkness);
            // Color Adjustment
            mainPostVolume.TryGet(out ColorAdjustments colorAdjustments);
            colorAdjustments.postExposure.value = -darkness;
            colorAdjustments.saturation.value = -16.0f * darkness;
            // White Balance
            mainPostVolume.TryGet(out WhiteBalance whiteBalance);
            whiteBalance.temperature.value = (brightness * 16.0f) - 8.0f - (darkness * 4.0f);
        }


#if UNITY_EDITOR
        public void Tests()
        {   
            Quaternion fakerot = quaternion.EulerXYZ(90, 0, WorldTime.TimeInMonth * 8.73283872628f * 6.0f - 90f);
            float fake = (1.0f - Mathf.Cos(Mathf.Deg2Rad * fakerot.eulerAngles.z)) * 0.5f;
            skyMaterial.SetFloat("_Cloud_Density", fake);
        }
#endif




        private void SetAmbient(Color color)
        {
            Shader.SetGlobalColor("_Ambient", color);
        }


    }

}
