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
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Particle FX", order = 361)]
    public class ParticleFX : FXProfile
    {

        public CozyParticles particleSystem;
        private CozyParticles runtimeRef;
        public bool autoScale;

        public override void PlayEffect(float intensity)
        {
            if (!runtimeRef)
                if (InitializeEffect(weatherSphere) == false)
                    return;

            if (autoScale)
                runtimeRef.transform.localScale = weatherSphere.transform.GetChild(0).localScale;

            if (intensity == 0)
            {
                runtimeRef.Stop();
                return;
            }

            runtimeRef.Play(transitionTimeModifier.Evaluate(intensity));

        }

        public override bool InitializeEffect(CozyWeather weather)
        {
            if (!Application.isPlaying)
                return false;

            base.InitializeEffect(weather);

            if (runtimeRef == null)
            {
                runtimeRef = weather.GetFXRuntimeRef<CozyParticles>(name);
                if (runtimeRef)
                    return true;

                runtimeRef = Instantiate(particleSystem).GetComponent<CozyParticles>();

                runtimeRef.gameObject.name = name;
                runtimeRef.transform.parent = weather.particleFXParent;
                runtimeRef.transform.localPosition = Vector3.zero;
                runtimeRef.transform.localRotation = Quaternion.identity;

                runtimeRef.SetupTriggers();
                if (autoScale)
                    runtimeRef.transform.localScale *= weather.transform.GetChild(0).localScale.x;
            }

            return true;

        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ParticleFX))]
    [CanEditMultipleObjects]
    public class E_ParticleFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("particleSystem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoScale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();

        }

        public override void RenderInWindow(Rect pos)
        {

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var propPosA = new Rect(pos.x, pos.y + space, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosB = new Rect(pos.x, pos.y + space * 2, pos.width, EditorGUIUtility.singleLineHeight);
            var propPosC = new Rect(pos.x, pos.y + space * 3, pos.width, EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("particleSystem"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("autoScale"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("transitionTimeModifier"));

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetLineHeight()
        {

            return 3;

        }

    }
#endif
}