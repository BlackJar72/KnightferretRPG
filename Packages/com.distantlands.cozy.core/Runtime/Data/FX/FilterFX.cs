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
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Filter FX", order = 361)]
    public class FilterFX : FXProfile
    {



        [Range(-1, 1)]
        public float filterSaturation;
        [Range(-1, 1)]
        public float filterValue;
        [ColorUsage(false, true)]
        public Color filterColor = Color.white;
        [ColorUsage(false, true)]
        public Color sunFilter = Color.white;
        [ColorUsage(false, true)]
        public Color cloudFilter = Color.white;
        CozyWeatherModule weatherModule;

        public override void PlayEffect(float weight)
        {

            if (!weatherSphere)
                if (InitializeEffect(null) == false)
                    return;

            weatherModule.filterSaturation = Mathf.Lerp(weatherModule.filterSaturation, filterSaturation, weight);
            weatherModule.filterValue = Mathf.Lerp(weatherModule.filterValue, filterValue, weight);
            weatherModule.filterColor = Color.Lerp(weatherModule.filterColor, filterColor, weight);
            weatherModule.sunFilter = Color.Lerp(weatherModule.sunFilter, sunFilter, weight);
            weatherModule.cloudFilter = Color.Lerp(weatherModule.cloudFilter, cloudFilter, weight);

        }


        public override bool InitializeEffect(CozyWeather weather)
        {

            weatherSphere = weather ? weather : CozyWeather.instance;

            if (!weatherSphere.weatherModule)
                return false;

            weatherModule = weatherSphere.weatherModule;

            return true;

        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FilterFX))]
    [CanEditMultipleObjects]
    public class E_FilterFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterSaturation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunFilter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudFilter"));
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
            var propPosF = new Rect(pos.x, pos.y + space * 6, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("filterSaturation"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("filterValue"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("filterColor"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("sunFilter"));
            EditorGUI.PropertyField(propPosE, serializedObject.FindProperty("cloudFilter"));
            EditorGUI.PropertyField(propPosF, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 6;

        }

    }
#endif
}