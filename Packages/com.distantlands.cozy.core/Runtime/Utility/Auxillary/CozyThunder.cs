// Distant Lands 2025.



using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif


namespace DistantLands.Cozy
{
    public class CozyThunder : MonoBehaviour
    {


        [SerializeField]
        private AudioClip[] m_ThunderSounds;
        [SerializeField]
        private AnimationCurve m_LightIntensity;
        [SerializeField]
        private Vector2 m_ThunderDelayRange;


        private Light m_Light;
        private AudioSource m_AudioSource;

        private float m_WakeTime;
        private float m_WakeAmount;
        private float m_ThunderDelay;



        // Start is called before the first frame update
        void Start()
        {

            m_WakeTime = Time.time;
            m_Light = GetComponentInChildren<Light>();
            m_AudioSource = GetComponentInChildren<AudioSource>();


            m_AudioSource.clip = m_ThunderSounds[Random.Range(0, m_ThunderSounds.Length)];
            m_ThunderDelay = Random.Range(m_ThunderDelayRange.x, m_ThunderDelayRange.y);

        }

        // Update is called once per frame
        void Update()
        {

            m_WakeAmount = Time.time - m_WakeTime;

            m_Light.intensity = m_LightIntensity.Evaluate(m_WakeAmount);

            if (m_WakeAmount > m_AudioSource.clip.length + m_ThunderDelay)
            {
                Destroy(gameObject);
                return;
            }

            if (m_WakeAmount > m_ThunderDelay && !m_AudioSource.isPlaying)
                m_AudioSource.Play();

        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(CozyThunder))]
    [CanEditMultipleObjects]
    public class E_Thunder : Editor
    {
        CozyThunder cozythunder;

        void OnEnable()
        {
            cozythunder = (CozyThunder)target;
        }

        public override void OnInspectorGUI()
        {
            if (cozythunder == null)
                if (target)
                    cozythunder = (CozyThunder)target;
                else
                    return;

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ThunderSounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ThunderDelayRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_LightIntensity"));
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}