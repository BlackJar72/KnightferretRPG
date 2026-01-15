//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Audio FX", order = 361)]
    public class AudioFX : FXProfile
    {

        public AudioClip clip;
        [Tooltip("The audio mixer group that the COZY weather audio FX will use.")]
        public AudioMixerGroup audioMixer;
        private AudioSource runtimeRef;
        public float maximumVolume = 1;

        public override void PlayEffect(float weight)
        {

            if (!runtimeRef)
                if (InitializeEffect(weatherSphere) == false)
                    return;

            if (weight == 0)
            {
                runtimeRef.volume = 0;
                runtimeRef.Stop();
                return;
            }


            if (!runtimeRef.isPlaying && runtimeRef.isActiveAndEnabled)
                runtimeRef.Play();

            runtimeRef.volume = maximumVolume * transitionTimeModifier.Evaluate(weight);

        }

        public override bool InitializeEffect(CozyWeather weather)
        {
            if (!Application.isPlaying)
                return false;

            base.InitializeEffect(weather);

            if (runtimeRef == null)
            {
                runtimeRef = weather.GetFXRuntimeRef<AudioSource>(name);
                if (runtimeRef)
                    return true;

                runtimeRef = new GameObject().AddComponent<AudioSource>();

                runtimeRef.gameObject.name = name;
                runtimeRef.transform.parent = weather.audioFXParent;
                runtimeRef.transform.localPosition = Vector3.zero;
                runtimeRef.transform.localRotation = Quaternion.identity;
                runtimeRef.clip = clip;
                runtimeRef.outputAudioMixerGroup = audioMixer;
                runtimeRef.playOnAwake = false;
                runtimeRef.volume = 0;
                runtimeRef.loop = true;
            }

            return true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioFX))]
    [CanEditMultipleObjects]
    public class E_AudioFX : E_FXProfile
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("clip"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioMixer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumVolume"));
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

            EditorGUI.PropertyField(propPosA, serializedObject.FindProperty("clip"));
            EditorGUI.PropertyField(propPosB, serializedObject.FindProperty("audioMixer"));
            EditorGUI.PropertyField(propPosC, serializedObject.FindProperty("maximumVolume"));
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