//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using OverrideData = DistantLands.Cozy.Overridable<float>;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Climate FX", order = 361)]
    public class ClimateFX : FXProfile
    {


        [OverrideRange(-50, 50)]
        public OverrideData temperatureOffset;
        [OverrideRange(-50, 50)]
        public OverrideData precipitationOffset;
        CozyClimateModule climate;

        public override void PlayEffect(float weight)
        {
            if (climate == null)
                if (InitializeEffect(null) == false)
                    return;

            climate.temperatureOffset += temperatureOffset * weight;
            climate.precipitationOffset += precipitationOffset * weight;

        }

        public override bool InitializeEffect(CozyWeather weather)
        {

            base.InitializeEffect(weather);
            if (weatherSphere.climateModule == null)
                return false;

            climate = weatherSphere.climateModule;

            return true;

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ClimateFX))]
    [CanEditMultipleObjects]
    public class E_CliamteFX : E_FXProfile
    {

        SerializedProperty temperatureOffset;
        SerializedProperty precipitationOffset;

        SerializedProperty transitionTimeModifier;

        void OnEnable()
        {
            temperatureOffset = serializedObject.FindProperty("temperatureOffset");
            precipitationOffset = serializedObject.FindProperty("precipitationOffset");
            transitionTimeModifier = serializedObject.FindProperty("transitionTimeModifier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(temperatureOffset);
            EditorGUILayout.PropertyField(precipitationOffset);
            EditorGUILayout.PropertyField(transitionTimeModifier);

            serializedObject.ApplyModifiedProperties();

        }

        public override void RenderInWindow(Rect pos)
        {

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, temperatureOffset);
            EditorGUI.PropertyField(propPosB, precipitationOffset);
            EditorGUI.PropertyField(propPosC, transitionTimeModifier);

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 3;

        }

    }
#endif
}