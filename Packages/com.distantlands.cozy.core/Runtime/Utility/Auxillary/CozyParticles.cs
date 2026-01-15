// Distant Lands 2025.



using DistantLands.Cozy.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


namespace DistantLands.Cozy
{
    public class CozyParticles : MonoBehaviour
    {

        private CozyWeather weatherSphere;
        [SerializeField]
        private VisualEffect[] m_VisualEffects;

        [SerializeField]
        private ParticleSystem[] m_ParticleSystems;

        [System.Serializable]
        public class ParticleType
        {
            public ParticleSystem particleSystem;
            public float emissionAmount;
        }

        [HideInInspector]
        public List<ParticleType> m_ParticleTypes;


        // Start is called before the first frame update
        void Awake()
        {

            weatherSphere = CozyWeather.instance;

            if (m_ParticleSystems.Length == 0)
                m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();

            if (m_VisualEffects.Length == 0)
                m_VisualEffects = GetComponentsInChildren<VisualEffect>();

            foreach (ParticleSystem i in m_ParticleSystems)
            {
                if (i == null)
                    continue;

                ParticleType j = new ParticleType
                {
                    particleSystem = i,
                    emissionAmount = i.emission.rateOverTime.constant
                };
                m_ParticleTypes.Add(j);
            }

            foreach (ParticleType i in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule k = i.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = k.rateOverTime;

                j.constant = 0;
                k.rateOverTime = j;
            }

            foreach (VisualEffect i in m_VisualEffects)
            {
                i.Stop();
            }
        }

        public void SetupTriggers()
        {
            foreach (ParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.TriggerModule triggers = particle.particleSystem.trigger;

                triggers.enter = ParticleSystemOverlapAction.Kill;
                triggers.inside = ParticleSystemOverlapAction.Kill;
                for (int j = 0; j < weatherSphere.cozyTriggers.Count; j++)
                {
                    triggers.SetCollider(j, weatherSphere.cozyTriggers[j]);
                }
            }

            /// NOTE: VFX Graph does not currently support triggers. Maybe take a look at this in the future

        }

        public void Play()
        {

            if (this == null)
                return;

            foreach (ParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule i = particle.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = i.rateOverTime;

                // j.constant = particle.emissionAmount * particleManager.multiplier;
                i.rateOverTime = j;
                if (particle.particleSystem.isStopped)
                    particle.particleSystem.Play();
            }

            foreach (VisualEffect particle in m_VisualEffects)
            {
                particle.Play();
            }
        }

        public void Stop()
        {

            if (m_ParticleTypes != null)
                foreach (ParticleType particle in m_ParticleTypes)
                {

                    if (particle.particleSystem != null)
                        if (particle.particleSystem.isPlaying)
                            particle.particleSystem.Stop();
                }

            foreach (VisualEffect particle in m_VisualEffects)
            {
                particle.Stop();
            }
        }

        public void Play(float weight)
        {

            if (this == null)
                return;

            foreach (ParticleType particle in m_ParticleTypes)
            {
                ParticleSystem.EmissionModule i = particle.particleSystem.emission;
                ParticleSystem.MinMaxCurve j = i.rateOverTime;

                j.constant = Mathf.Lerp(0, particle.emissionAmount, weight);
                i.rateOverTime = j;
                if (particle.particleSystem.isStopped)
                    particle.particleSystem.Play();
            }

            foreach (VisualEffect particle in m_VisualEffects)
            {
                if (weight > 0.5f)
                    particle.Play();
                else
                    particle.Stop();
            }

        }
    }
}