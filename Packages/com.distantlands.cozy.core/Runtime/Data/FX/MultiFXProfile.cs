//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Multi FX", order = 361)]
    public class MultiFXProfile : FXProfile
    {

        public CozyWeather weather;

        [System.Serializable]
        public class MultiFXType
        {
            public FXProfile FX;
            public ChanceEffector intensityCurve;
        }

        [MultiAudio]
        public List<MultiFXType> multiFX;
        
        [MultiAudio]
        public MultiFXType test;

        public override void PlayEffect(float weight)
        {

            if (weather == null)
                weather = CozyWeather.instance;

            foreach (MultiFXType i in multiFX)
            {
                i.FX.PlayEffect(i.intensityCurve.GetChance(weather) * weight);
            }
        }

        public override bool InitializeEffect(CozyWeather weather)
        {
            if (weather == null)
                weather = CozyWeather.instance;

            foreach (MultiFXType i in multiFX)
            {
                i.FX.InitializeEffect(weather);
            }

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MultiFXProfile))]
    [CanEditMultipleObjects]
    public class E_MultiFXProfile : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void RenderInWindow(Rect pos)
        {

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("multiFX"), true);

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 1 + (serializedObject.FindProperty("multiFX").isExpanded ? (serializedObject.FindProperty("multiFX").arraySize * 2) + 1 : 0);

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("multiFX"));

            serializedObject.ApplyModifiedProperties();

        }

    }
#endif
}