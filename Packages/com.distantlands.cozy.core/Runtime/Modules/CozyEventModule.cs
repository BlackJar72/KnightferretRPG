//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy
{
    public class CozyEventModule : CozyBiomeModuleBase<CozyEventModule>
    {
        [CozySearchable]
        public UnityEvent onDawn;
        [CozySearchable]
        public UnityEvent onMorning;
        [CozySearchable]
        public UnityEvent onDay;
        [CozySearchable]
        public UnityEvent onAfternoon;
        [CozySearchable]
        public UnityEvent onEvening;
        [CozySearchable]
        public UnityEvent onTwilight;
        [CozySearchable]
        public UnityEvent onNight;
        [CozySearchable]
        public UnityEvent onNewMinute;
        [CozySearchable]
        public UnityEvent onNewHour;
        [CozySearchable]
        public UnityEvent onNewDay;
        [CozySearchable]
        public UnityEvent onNewYear;
        [CozySearchable]
        public UnityEvent onWeatherProfileChange;

        [System.Serializable]
        public class CozyEvent
        {

            public EventFX fxReference;
            public UnityEvent onPlay;
            public UnityEvent onStop;

        }

        [CozySearchable]
        public CozyEvent[] cozyEvents;
        
        public bool inBiome = false;
        public UnityEvent onEnterBiome;
        public UnityEvent onExitBiome;
        public UnityEvent whileInBiome;

        public override void InitializeModule()
        {
            if (!enabled)
                return;
            
            base.InitializeModule();

            if (GetComponent<CozyWeather>())
            {

                GetComponent<CozyWeather>().InitializeModule(typeof(CozyEventModule));
                DestroyImmediate(this);
                Debug.LogWarning("Add modules in the settings tab in COZY 2!");
                return;

            }

            isBiomeModule = GetComponent<CozyBiome>();
            if (isBiomeModule)
            {
                return;
            }

            if (Application.isPlaying)
            {
                foreach (CozyEvent i in cozyEvents)
                {
                    if (i.fxReference)
                    {
                        i.fxReference.onCall += i.onPlay.Invoke;
                        i.fxReference.onEnd += i.onStop.Invoke;
                    }
                }

                StartCoroutine(Refresh());
            }

        }

        public override void DeinitializeModule()
        {
            base.DeinitializeModule();

            if (Application.isPlaying)
            {
                foreach (CozyEvent i in cozyEvents)
                {
                    if (i.fxReference)
                    {
                        i.fxReference.onCall -= i.onPlay.Invoke;
                        i.fxReference.onEnd -= i.onStop.Invoke;
                    }
                }

                CozyWeather.Events.onDawn -= onDawn.Invoke;
                CozyWeather.Events.onMorning -= onMorning.Invoke;
                CozyWeather.Events.onDay -= onDay.Invoke;
                CozyWeather.Events.onAfternoon -= onAfternoon.Invoke;
                CozyWeather.Events.onEvening -= onEvening.Invoke;
                CozyWeather.Events.onTwilight -= onTwilight.Invoke;
                CozyWeather.Events.onNight -= onNight.Invoke;
                CozyWeather.Events.onNewMinute -= onNewMinute.Invoke;
                CozyWeather.Events.onNewHour -= onNewHour.Invoke;
                CozyWeather.Events.onNewDay -= onNewDay.Invoke;
                CozyWeather.Events.onNewYear -= onNewYear.Invoke;
                CozyWeather.Events.onWeatherChange -= onWeatherProfileChange.Invoke;

            }
        }

        public IEnumerator Refresh()
        {

            yield return new WaitForEndOfFrame();

            CozyWeather.Events.onDawn += onDawn.Invoke;
            CozyWeather.Events.onMorning += onMorning.Invoke;
            CozyWeather.Events.onDay += onDay.Invoke;
            CozyWeather.Events.onAfternoon += onAfternoon.Invoke;
            CozyWeather.Events.onEvening += onEvening.Invoke;
            CozyWeather.Events.onTwilight += onTwilight.Invoke;
            CozyWeather.Events.onNight += onNight.Invoke;
            CozyWeather.Events.onNewMinute += onNewMinute.Invoke;
            CozyWeather.Events.onNewHour += onNewHour.Invoke;
            CozyWeather.Events.onNewDay += onNewDay.Invoke;
            CozyWeather.Events.onNewYear += onNewYear.Invoke;
            CozyWeather.Events.onWeatherChange += onWeatherProfileChange.Invoke;

        }

        public void LogConsoleEvent()
        {

            Debug.Log("Test Event Passed.");

        }

        public void LogConsoleEvent(string log)
        {

            Debug.Log($"Test Event Passed. Log: {log}");

        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (!isBiomeModule)
            {
                ComputeBiomeWeights();
                return;
            }

            if (weight == 1)
            {
                whileInBiome.Invoke();
                if (inBiome != true)
                {
                    inBiome = true;
                    onEnterBiome.Invoke();
                }
            }
            if (weight == 0 && inBiome != false)
            {
                inBiome = false;
                onExitBiome.Invoke();
            }

        }

    }

}