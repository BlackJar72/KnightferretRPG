//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using UnityEngine;
using DistantLands.Cozy.Data;
using System.Collections.Generic;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyWindModule : CozyModule
    {

        [CozySearchable]
        public WindFX defaultWindProfile;
        [CozySearchable]
        public WindZone windZone;

        public float windSpeed;
        public float windChangeSpeed;
        public float windAmount;
        public float windGusting;
        private Vector3 m_WindDirection;
        private float m_Seed;
        [Tooltip("Multiplies the total wind power by a coefficient.")]
        [Range(0, 2)]
        [CozySearchable]
        public float windMultiplier = 1;
        [CozySearchable]
        public bool useWindzone = true;
        [CozySearchable]
        public bool useShaderWind = true;
        private float m_WindTime;
        public List<WindFX> windFXes = new List<WindFX>();

        public Vector3 WindDirection
        {
            get { return m_WindDirection; }
            set { m_WindDirection = WindDirection; }

        }



        void Start()
        {
            weatherSphere.windModule = this;

            if (!defaultWindProfile)
                defaultWindProfile = (WindFX)Resources.Load("Default Profiles/Default Wind");

            m_WindTime = 0;
            m_Seed = Random.value * 1000;


        }

        public override void CozyUpdateLoop()
        {

            if (defaultWindProfile == null)
            {
                Debug.LogWarning("Default wind profile is required for the COZY Wind Module");
                return;
            }

            float i = 360 * Mathf.PerlinNoise(m_Seed, Time.time * windChangeSpeed / 100000);
            m_WindDirection = new Vector3(Mathf.Sin(i), 0, Mathf.Cos(i));


            if (useWindzone)
            {

                if (windZone)
                {
                    windZone.transform.LookAt(windZone.transform.position + m_WindDirection, Vector3.up);
                    windZone.windMain = windAmount * windMultiplier;
                    windZone.windPulseMagnitude = windGusting;
                    windZone.windPulseFrequency = windSpeed;
                }
            }

            m_WindTime += Time.deltaTime * windSpeed;

            if (useShaderWind)
            {
                Shader.SetGlobalFloat("CZY_WindTime", m_WindTime);
                Shader.SetGlobalVector("CZY_WindDirection", m_WindDirection * windAmount * windMultiplier);
            }

        }

        public override void FrameReset()
        {
            if (defaultWindProfile)
            {
                windSpeed = defaultWindProfile.windSpeed;
                windAmount = defaultWindProfile.windAmount;
                windGusting = defaultWindProfile.windGusting;
                windChangeSpeed = defaultWindProfile.windChangeSpeed;
            }
        }

        public override void DeinitializeModule()
        {
            base.DeinitializeModule();

            Shader.SetGlobalFloat("CZY_WindTime", 0);
            Shader.SetGlobalVector("CZY_WindDirection", Vector3.zero);

        }

        public float WindSpeedInKnots => windAmount * windSpeed * windMultiplier * 10f;
    }

}