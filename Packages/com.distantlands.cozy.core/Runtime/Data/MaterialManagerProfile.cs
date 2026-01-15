//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;



namespace DistantLands.Cozy.Data
{

    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/Material Manager Profile", order = 361)]
    public class MaterialManagerProfile : CozyProfile
    {


        public Texture snowTexture;
        public float snowNoiseSize = 10;
        public Color snowColor = Color.white;
        public float puddleScale = 2;



        [System.Serializable]
        public class ModulatedValue
        {
            public enum ModulationSource { dayPercent, yearPercent, precipitation, temperature, snowAmount, rainAmount }
            public enum ModulationTarget { terrainLayerColor, terrainLayerTint, materialColor, materialValue, globalColor, globalValue }
            [Tooltip("The source that will modulate the target.")]
            public ModulationSource modulationSource;
            [Tooltip("The target type that will be modulated.")]
            public ModulationTarget modulationTarget;
            [Tooltip("The gradient that will pass a color to the modulation target based on the modulation source.")]
            public Gradient mappedGradient;
            [Tooltip("The curve that will pass a float value to the modulation target based on the modulation source.")]
            public AnimationCurve mappedCurve;

            [Tooltip("The terrain layer that this profile impacts.")]
            public TerrainLayer targetLayer;
            [Tooltip("The material that this profile impacts.")]
            public Material targetMaterial;

            public string targetVariableName;


        }

        [ModulatedProperty]
        public ModulatedValue[] modulatedValues;

    }


}