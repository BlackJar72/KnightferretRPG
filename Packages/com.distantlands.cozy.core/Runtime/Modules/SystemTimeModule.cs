//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System;
using UnityEngine;

namespace DistantLands.Cozy
{
    public class SystemTimeModule : CozyTimeModule
    {

        [MeridiemTimeAttribute]
        [SerializeField]
        private float m_SystemTime = 0.5f;
        [SerializeField]
        [CozySearchable]
        public bool pauseTime;
        [Tooltip("How many times should the COZY day complete per real world day.")]
        [CozySearchable]
        public float timeMultiplier = 1;
        [Tooltip("How many times should the COZY year complete per real world year.")]
        [CozySearchable]
        public float dateMultiplier = 1;

        public enum TimeGatherMode { Local, UTC }

        [CozySearchable]
        public TimeGatherMode timeGatherMode;
        [Tooltip("Adds an offset to the gathered time in hours.")]
        [CozySearchable]
        public float hourOffset;

        internal override bool CheckIfModuleCanBeAdded(out string warning)
        {
            if (weatherSphere.moduleHolder.GetComponents<CozyTimeModule>().Length != 1)
            {
                warning = "Time Module";
                return false;
            }
            warning = "";
            return true;
        }

        public void Update()
        {
            if (weatherSphere.timeModule == null)
                weatherSphere.timeModule = this;

            if (!pauseTime)
            {
                if (timeGatherMode == TimeGatherMode.Local)
                {
                    m_SystemTime = (hourOffset * 3600000 + (float)DateTime.Now.TimeOfDay.TotalMilliseconds) * timeMultiplier / 86400000 % 1;
                    yearPercentage = (float)DateTime.Now.DayOfYear / 365 * dateMultiplier % 1;
                }
                else
                {
                    m_SystemTime = (hourOffset * 3600000 + (float)DateTime.UtcNow.TimeOfDay.TotalMilliseconds) * timeMultiplier / 86400000 % 1;
                    yearPercentage = (float)DateTime.UtcNow.DayOfYear / 365 * dateMultiplier % 1;
                }
                currentTime = m_SystemTime;

            }
        }

        public new float modifiedTimeSpeed
        {
            get
            {
                return timeMultiplier / 86400;
            }
        }
    }

}