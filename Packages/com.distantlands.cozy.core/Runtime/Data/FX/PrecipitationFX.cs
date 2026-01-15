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
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Precipitation FX", order = 361)]
    public class PrecipitationFX : FXProfile
    {



        [Range(0, 0.05f)]
        public float rainAccumulationSpeed;
        [Range(0, 1f)]
        public float maximumRainAmount;
        [Range(0, 0.05f)]
        public float snowAccumulationSpeed;
        [Range(0, 1f)]
        public float maximumSnowAmount;
        public float weight;
        CozyWeather weather;
        CozyClimateModule climateModule;

        public override void PlayEffect(float i)
        {
            if (!weather)
                if (InitializeEffect(null) == false)
                    return;

            climateModule.snowSpeed += snowAccumulationSpeed * Mathf.Clamp01(transitionTimeModifier.Evaluate(i)) * (climateModule.snowAmount < maximumSnowAmount ? 1 : 0);
            climateModule.rainSpeed += rainAccumulationSpeed * Mathf.Clamp01(transitionTimeModifier.Evaluate(i)) * (climateModule.groundwaterAmount < maximumRainAmount ? 1 : 0);
            
        }

        public override bool InitializeEffect(CozyWeather weather)
        {

            weatherSphere = weather ? weather : CozyWeather.instance;

            if (!weatherSphere.climateModule)
                return false;

            climateModule = weatherSphere.climateModule;

            return true;

        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PrecipitationFX))]
    [CanEditMultipleObjects]
    public class E_PrecipitationFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("rainAccumulationSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumRainAmount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("snowAccumulationSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSnowAmount"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

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

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("rainAccumulationSpeed"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("maximumRainAmount"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("snowAccumulationSpeed"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("maximumSnowAmount"));
            EditorGUI.PropertyField(propPosE, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 5;

        }

    }
#endif
}