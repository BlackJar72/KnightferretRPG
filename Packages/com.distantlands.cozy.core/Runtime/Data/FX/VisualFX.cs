//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using UnityEngine;
#if COZY_URP
using UnityEngine.Rendering;
#elif UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Post Processing FX", order = 361)]
    public class VisualFX : FXProfile
    {

        public int layer;
        public float priority = 100;
#if COZY_URP 
        public VolumeProfile effectSettings;
        Volume _volume;

#elif UNITY_POST_PROCESSING_STACK_V2
        public PostProcessProfile effectSettings;
        PostProcessVolume _volume;
#endif

        public override void PlayEffect(float i)
        {
#if UNITY_POST_PROCESSING_STACK_V2 || COZY_URP
            if (!_volume)
                if (!InitializeEffect(weatherSphere))
                    return;

            _volume.weight = Mathf.Clamp01(transitionTimeModifier.Evaluate(i));
#endif
        }
        public override bool InitializeEffect(CozyWeather weather)
        {
            if (!Application.isPlaying)
                return false;

            base.InitializeEffect(weather);

#if UNITY_POST_PROCESSING_STACK_V2 || COZY_URP
            if (_volume)
                return true;

            if (_volume == null)
            {
#if COZY_URP 
                if (weather.GetFXRuntimeRef<Volume>(name))
                    _volume = weather.GetFXRuntimeRef<Volume>(name);
#elif UNITY_POST_PROCESSING_STACK_V2
                _volume = weather.GetFXRuntimeRef<PostProcessVolume>(name);
#endif
                if (_volume)
                    return true;

#if COZY_URP 
                _volume = new GameObject().AddComponent<Volume>();
#elif  UNITY_POST_PROCESSING_STACK_V2
                _volume = new GameObject().AddComponent<PostProcessVolume>();
#endif
                _volume.gameObject.name = name;
                _volume.transform.parent = weather.visualFXParent;
                _volume.transform.position = Vector3.zero;
                _volume.transform.rotation = Quaternion.identity;
                _volume.profile = effectSettings;
                _volume.priority = priority;
                _volume.weight = 0;
                _volume.isGlobal = true;
                _volume.gameObject.layer = layer;

                return true;
            }
#endif

            return false;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(VisualFX))]
    [CanEditMultipleObjects]
    public class E_VisualFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LayerField(new GUIContent("Volume Layer"), serializedObject.FindProperty("layer").intValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"), new GUIContent("Priority"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectSettings"), new GUIContent("Post Processing Profile"));
            EditorGUILayout.Space();
            if (serializedObject.FindProperty("effectSettings").objectReferenceValue)
                CreateEditor(serializedObject.FindProperty("effectSettings").objectReferenceValue).OnInspectorGUI();

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

            EditorGUI.LayerField(propPosA, new GUIContent("Volume Layer"), serializedObject.FindProperty("layer").intValue);
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("priority"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("effectSettings"));

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