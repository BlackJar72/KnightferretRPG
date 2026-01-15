//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DistantLands.Cozy.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Distant Lands/Cozy/FX/Event FX", order = 361)]
    public class EventFX : FXProfile
    {

        public CozyEventModule events;

        public bool isPlaying;
        public delegate void OnCall();
        public event OnCall onCall;
        public void RaiseOnCall()
        {
            onCall?.Invoke();
        }
        public delegate void OnEnd();
        public event OnEnd onEnd;
        public void RaiseOnEnd()
        {
            onEnd?.Invoke();
        }

        public void PlayEffect()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                onCall?.Invoke();
            }
        }

        public override void PlayEffect(float weight)
        {
            if (weight > 0.5f)
                PlayEffect();
            else
                StopEffect();
        }

        public void StopEffect()
        {
            if (isPlaying)
            {
                isPlaying = false;
                onEnd?.Invoke();
            }
        }

        public override bool InitializeEffect(CozyWeather weather)
        {

            base.InitializeEffect(weather);

            if (!weatherSphere.GetModule<CozyEventModule>())
                return false;

            events = weatherSphere.GetModule<CozyEventModule>();
            return true;

        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(EventFX))]
    [CanEditMultipleObjects]
    public class E_EventFX : E_FXProfile
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox("No other properties to adjust! Set events in the Cozy Event Module!", MessageType.Info);

            serializedObject.ApplyModifiedProperties();

        }

        public override void RenderInWindow(Rect pos)
        {

        }

        public override float GetLineHeight()
        {

            return 0;

        }

    }
#endif
}