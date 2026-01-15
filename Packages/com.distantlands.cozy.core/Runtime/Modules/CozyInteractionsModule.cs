//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using DistantLands.Cozy.Data;
using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy
{

    [ExecuteAlways]
    public class CozyInteractionsModule : CozyModule
    {

        [CozySearchable(true)]
        public MaterialManagerProfile profile;
        // public List<PrecipitationFX> precipitationFXes = new List<PrecipitationFX>();

        // Start is called before the first frame update
        void Awake()
        {
            if (profile == null)
                return;

            SetupStaticGlobalVariables();

        }

        // Update is called once per frame                              
        public override void CozyUpdateLoop()
        {
            if (weatherSphere == null)
                base.InitializeModule();

            if (profile == null)
                return;

            if (CozyWeather.FreezeUpdateInEditMode && !Application.isPlaying)
                return;

            SetupStaticGlobalVariables();

            foreach (MaterialManagerProfile.ModulatedValue i in profile.modulatedValues)
            {
                switch (i.modulationTarget)
                {
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.globalColor:
                        Shader.SetGlobalColor(i.targetVariableName, i.mappedGradient.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.globalValue:
                        Shader.SetGlobalFloat(i.targetVariableName, i.mappedCurve.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.materialColor:
                        if (i.targetMaterial)
                            i.targetMaterial.SetColor(i.targetVariableName, i.mappedGradient.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.materialValue:
                        if (i.targetMaterial)
                            i.targetMaterial.SetFloat(i.targetVariableName, i.mappedCurve.Evaluate(GetPercentage(i.modulationSource)));
                        break;
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.terrainLayerColor:
                        if (i.targetLayer)
                            i.targetLayer.specular = i.mappedGradient.Evaluate(GetPercentage(i.modulationSource));
                        break;
                    case MaterialManagerProfile.ModulatedValue.ModulationTarget.terrainLayerTint:
                        if (i.targetLayer)
                            i.targetLayer.diffuseRemapMax = i.mappedGradient.Evaluate(GetPercentage(i.modulationSource));
                        break;

                }
            }

        }

        float GetPercentage(MaterialManagerProfile.ModulatedValue.ModulationSource modulationSource)
        {

            float i = 0;

            switch (modulationSource)
            {
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.dayPercent):
                    if (weatherSphere.timeModule)
                        i = weatherSphere.timeModule.currentTime;
                    break;
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.precipitation):
                    if (weatherSphere.climateModule)
                        i = Mathf.Clamp01(weatherSphere.climateModule.currentPrecipitation / 100);
                    break;
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.rainAmount):
                    if (weatherSphere.climateModule)
                        i = weatherSphere.climateModule.groundwaterAmount;
                    break;
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.snowAmount):
                    if (weatherSphere.climateModule)
                        i = weatherSphere.climateModule.snowAmount;
                    break;
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.temperature):
                    if (weatherSphere.climateModule)
                        i = Mathf.Clamp01(weatherSphere.climateModule.GetTemperature() / 100);
                    break;
                case (MaterialManagerProfile.ModulatedValue.ModulationSource.yearPercent):
                    if (weatherSphere.timeModule)
                        i = weatherSphere.timeModule.yearPercentage;
                    break;

            }

            return i;

        }

        public void SetupStaticGlobalVariables()
        {

            Shader.SetGlobalFloat("CZY_SnowScale", profile.snowNoiseSize);
            Shader.SetGlobalTexture("CZY_SnowTexture", profile.snowTexture);
            Shader.SetGlobalColor("CZY_SnowColor", profile.snowColor);
            Shader.SetGlobalFloat("CZY_PuddleScale", profile.puddleScale);


        }

    }
}