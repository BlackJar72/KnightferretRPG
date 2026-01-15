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
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Wind FX", order = 361)]
    public class WindFX : FXProfile
    {



        [Range(0, 2)]
        public float windAmount;
        [Range(0, 2)]
        public float windSpeed;
        [Range(0, 2)]
        public float windGusting;
        [Range(0, 10)]
        public float windChangeSpeed = 1;
        CozyWindModule windModule;

        public override void PlayEffect(float weight)
        {
            if (!weatherSphere)
                return;

            if (!windModule)
            {
                if (!InitializeEffect(weatherSphere))
                    return;
            }

            windModule.windAmount += windAmount * weight;
            windModule.windGusting += windGusting * weight;
            windModule.windSpeed += windSpeed * weight;
            windModule.windChangeSpeed += windChangeSpeed * weight;
        }

        public override bool InitializeEffect(CozyWeather weather)
        {

            weatherSphere = weather ? weather : CozyWeather.instance;

            if (!weatherSphere.windModule)
                return false;

            windModule = weatherSphere.windModule;

            return true;

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WindFX))]
    [CanEditMultipleObjects]
    public class E_WindFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("windAmount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windGusting"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("windChangeSpeed"));
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

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("windAmount"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("windSpeed"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("windGusting"));

            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 4;

        }

    }
#endif
}