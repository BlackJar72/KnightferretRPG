//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Thunder FX", order = 361)]
    public class ThunderFX : FXProfile
    {

        public Vector2 timeBetweenStrikes;
        public GameObject thunderPrefab = null;
        public float weight;
        public CozyThunderManager runtimeRef;
        public float minimumDistance = 700;
        public float maximumDistance = 1200;
        public float minScreenXmultiplier = 0.1f;
        public float maxScreenXmultiplier = 0.9f;
        public float minScreenYmultiplier = 0.0f;
        public float maxScreenYmultiplier = 0.1f;
        [Range(0, 1)]
        [Tooltip("What percentage of the time should the lightning and thunder be forced to spawn in the camera's view?")]
        public float spawnInFrustumPercentage = 0.5f;

        public override void PlayEffect(float weight)
        {

            if (!runtimeRef)
                if (InitializeEffect(weatherSphere) == false)
                    return;

            runtimeRef.PlayEffect(transitionTimeModifier.Evaluate(weight));

        }

        public override bool InitializeEffect(CozyWeather weather)
        {
            if (!Application.isPlaying)
                return false;

            base.InitializeEffect(weather);

            if (runtimeRef == null)
            {
                if (weather.GetFXRuntimeRef<CozyThunderManager>(name))
                {
                    runtimeRef = weather.GetFXRuntimeRef<CozyThunderManager>(name);
                    return true;
                }

                runtimeRef = new GameObject().AddComponent<CozyThunderManager>();

                runtimeRef.gameObject.name = name;
                runtimeRef.transform.parent = weather.thunderFXParent;
                runtimeRef.transform.localPosition = Vector3.zero;
                runtimeRef.transform.localRotation = Quaternion.identity;
                runtimeRef.weatherSphere = weather;
                runtimeRef.thunderFX = this;

            }

            return true;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ThunderFX))]
    [CanEditMultipleObjects]
    public class E_ThunderFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("thunderPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeBetweenStrikes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnInFrustumPercentage"));

            float min = serializedObject.FindProperty("minScreenXmultiplier").floatValue;
            float max = serializedObject.FindProperty("maxScreenXmultiplier").floatValue;

            EditorGUILayout.MinMaxSlider("Frustum Placement X", ref min, ref max, 0, 1);

            serializedObject.FindProperty("minScreenXmultiplier").floatValue = min;
            serializedObject.FindProperty("maxScreenXmultiplier").floatValue = max;

            min = serializedObject.FindProperty("minScreenYmultiplier").floatValue;
            max = serializedObject.FindProperty("maxScreenYmultiplier").floatValue;

            EditorGUILayout.MinMaxSlider("Frustum Placement Y", ref min, ref max, 0, 1);

            serializedObject.FindProperty("minScreenYmultiplier").floatValue = min;
            serializedObject.FindProperty("maxScreenYmultiplier").floatValue = max;


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

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("thunderPrefab"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("timeBetweenStrikes"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("minimumDistance"));
            EditorGUI.PropertyField(propPosD, serializedObject.FindProperty("maximumDistance"));
            EditorGUI.PropertyField(propPosE, serializedObject.FindProperty("spawnInFrustumPercentage"));

            float min = serializedObject.FindProperty("minScreenXmultiplier").floatValue;
            float max = serializedObject.FindProperty("maxScreenXmultiplier").floatValue;

            EditorGUI.MinMaxSlider(propPosF, "Frustum Placement X", ref min, ref max, 0, 1);

            serializedObject.FindProperty("minScreenXmultiplier").floatValue = min;
            serializedObject.FindProperty("maxScreenXmultiplier").floatValue = max;

            min = serializedObject.FindProperty("minScreenYmultiplier").floatValue;
            max = serializedObject.FindProperty("maxScreenYmultiplier").floatValue;

            EditorGUI.MinMaxSlider(propPosG, "Frustum Placement Y", ref min, ref max, 0, 1);

            serializedObject.FindProperty("minScreenYmultiplier").floatValue = min;
            serializedObject.FindProperty("maxScreenYmultiplier").floatValue = max;

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 7;

        }

    }
#endif
}