//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Cloud FX", order = 361)]
    public class CloudFX : FXProfile
    {


        [Tooltip("Multiplier for cumulus clouds.")]
        [OverrideRange(0, 2)]
        public Overridable<float> cumulusCoverage = 1;
        [Tooltip("Multiplier for altocumulus clouds.")]
        [OverrideRange(0, 2)]
        public Overridable<float> altocumulusCoverage = 0;
        [Tooltip("Multiplier for chemtrails.")]
        [OverrideRange(0, 2)]
        public Overridable<float> chemtrailCoverage = 0;
        [Tooltip("Multiplier for cirrostratus clouds.")]
        [OverrideRange(0, 2)]
        public Overridable<float> cirrostratusCoverage = 0;
        [Tooltip("Multiplier for cirrus clouds.")]
        [OverrideRange(0, 2)]
        public Overridable<float> cirrusCoverage = 0;
        [Tooltip("Multiplier for nimbus clouds.")]
        [OverrideRange(0, 2)]
        public Overridable<float> nimbusCoverage = 0;
        [Tooltip("Variation for nimbus clouds.")]
        [OverrideRange(0, 1)]
        public Overridable<float> nimbusVariation = 0.9f;
        [Tooltip("Height mask effect for nimbus clouds.")]
        [OverrideRange(0, 1)]
        public Overridable<float> nimbusHeightEffect = 1;

        [Tooltip("Starting height for cloud border.")]
        [OverrideRange(0, 1)]
        public Overridable<float> borderHeight = 0.5f;
        [Tooltip("Variation for cloud border.")]
        [OverrideRange(0, 1)]
        public Overridable<float> borderVariation = 0.9f;
        [Tooltip("Multiplier for the border. Values below zero clip the clouds whereas values above zero add clouds.")]
        [OverrideRange(-1, 1)]
        public Overridable<float> borderEffect = 1;
        [Tooltip("Controls the average density of the fog.")]
        [OverrideRange(0, 5)]
        public Overridable<float> fogDensity = 1;

        CozyWeatherModule weatherModule;



        public override void PlayEffect(float weight)
        {

            if (!weatherModule)
                if (InitializeEffect(null) == false)
                    return;

            weatherModule.cumulus = Mathf.Clamp(weatherModule.cumulus + cumulusCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.cirrus = Mathf.Clamp(weatherModule.cirrus + cirrusCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.altocumulus = Mathf.Clamp(weatherModule.altocumulus + altocumulusCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.cirrostratus = Mathf.Clamp(weatherModule.cirrostratus + cirrostratusCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.chemtrails = Mathf.Clamp(weatherModule.chemtrails + chemtrailCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.nimbus = Mathf.Clamp(weatherModule.nimbus + nimbusCoverage * transitionTimeModifier.Evaluate(weight), 0, 2);
            weatherModule.nimbusHeight = Mathf.Clamp(weatherModule.nimbusHeight + nimbusHeightEffect * transitionTimeModifier.Evaluate(weight), 0, 1);
            weatherModule.nimbusVariation = Mathf.Clamp(weatherModule.nimbusVariation + nimbusVariation * transitionTimeModifier.Evaluate(weight), 0, 1);
            weatherModule.borderHeight = borderHeight ? Mathf.Lerp(weatherModule.borderHeight, borderHeight, transitionTimeModifier.Evaluate(weight)) : weatherModule.borderHeight;
            weatherModule.borderEffect = borderEffect ? Mathf.Lerp(weatherModule.borderEffect, borderEffect, transitionTimeModifier.Evaluate(weight)) : weatherModule.borderEffect;
            weatherModule.borderVariation = borderVariation ? Mathf.Lerp(weatherModule.borderVariation, borderVariation, transitionTimeModifier.Evaluate(weight)) : weatherModule.borderVariation;
            weatherModule.fogDensity = fogDensity ? Mathf.Clamp(weatherModule.fogDensity + fogDensity * transitionTimeModifier.Evaluate(weight), 0, 5) : weatherModule.fogDensity;
            

        }

        public override bool InitializeEffect(CozyWeather weather)
        {

            base.InitializeEffect(weather);

            if (!weatherSphere.weatherModule)
                return false;

            weatherModule = weatherSphere.weatherModule;

            return true;

        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CloudFX))]
    [CanEditMultipleObjects]
    public class E_CloudFX : E_FXProfile
    {

        SerializedProperty cumulusCoverage;
        SerializedProperty altocumulusCoverage;
        SerializedProperty chemtrailCoverage;
        SerializedProperty cirrostratusCoverage;
        SerializedProperty cirrusCoverage;
        SerializedProperty nimbusCoverage;
        SerializedProperty nimbusVariation;
        SerializedProperty nimbusHeightEffect;
        SerializedProperty borderHeight;
        SerializedProperty borderVariation;
        SerializedProperty borderEffect;
        SerializedProperty fogDensity;
        SerializedProperty transitionTimeModifier;

        void OnEnable()
        {
            cumulusCoverage = serializedObject.FindProperty("cumulusCoverage");
            altocumulusCoverage = serializedObject.FindProperty("altocumulusCoverage");
            chemtrailCoverage = serializedObject.FindProperty("chemtrailCoverage");
            cirrostratusCoverage = serializedObject.FindProperty("cirrostratusCoverage");
            cirrusCoverage = serializedObject.FindProperty("cirrusCoverage");
            nimbusCoverage = serializedObject.FindProperty("nimbusCoverage");
            nimbusVariation = serializedObject.FindProperty("nimbusVariation");
            nimbusHeightEffect = serializedObject.FindProperty("nimbusHeightEffect");
            borderHeight = serializedObject.FindProperty("borderHeight");
            borderVariation = serializedObject.FindProperty("borderVariation");
            borderEffect = serializedObject.FindProperty("borderEffect");
            fogDensity = serializedObject.FindProperty("fogDensity");
            transitionTimeModifier = serializedObject.FindProperty("transitionTimeModifier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cumulusCoverage);
            EditorGUILayout.PropertyField(altocumulusCoverage);
            EditorGUILayout.PropertyField(chemtrailCoverage);
            EditorGUILayout.PropertyField(cirrostratusCoverage);
            EditorGUILayout.PropertyField(cirrusCoverage);
            EditorGUILayout.PropertyField(nimbusCoverage);
            EditorGUILayout.PropertyField(nimbusVariation);
            EditorGUILayout.PropertyField(nimbusHeightEffect);
            EditorGUILayout.PropertyField(borderHeight);
            EditorGUILayout.PropertyField(borderVariation);
            EditorGUILayout.PropertyField(borderEffect);
            EditorGUILayout.PropertyField(fogDensity);
            EditorGUILayout.PropertyField(transitionTimeModifier);

            serializedObject.ApplyModifiedProperties();

        }

        public override void RenderInWindow(Rect pos)
        {

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosD = new Rect(pos.x, pos.y + space * 4, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosE = new Rect(pos.x, pos.y + space * 5, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosF = new Rect(pos.x, pos.y + space * 6, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosG = new Rect(pos.x, pos.y + space * 7, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosH = new Rect(pos.x, pos.y + space * 8, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosI = new Rect(pos.x, pos.y + space * 9, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosJ = new Rect(pos.x, pos.y + space * 10, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosK = new Rect(pos.x, pos.y + space * 11, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosL = new Rect(pos.x, pos.y + space * 12, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosM = new Rect(pos.x, pos.y + space * 13, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, cumulusCoverage);
            EditorGUI.PropertyField(propPosB, altocumulusCoverage);
            EditorGUI.PropertyField(propPosC, chemtrailCoverage);
            EditorGUI.PropertyField(propPosD, cirrostratusCoverage);
            EditorGUI.PropertyField(propPosE, cirrusCoverage);
            EditorGUI.PropertyField(propPosF, nimbusCoverage);
            EditorGUI.PropertyField(propPosG, nimbusVariation);
            EditorGUI.PropertyField(propPosH, nimbusHeightEffect);
            EditorGUI.PropertyField(propPosI, borderHeight);
            EditorGUI.PropertyField(propPosJ, borderVariation);
            EditorGUI.PropertyField(propPosK, borderEffect);
            EditorGUI.PropertyField(propPosL, fogDensity);
            EditorGUI.PropertyField(propPosM, transitionTimeModifier);

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 13;

        }

    }
#endif
}