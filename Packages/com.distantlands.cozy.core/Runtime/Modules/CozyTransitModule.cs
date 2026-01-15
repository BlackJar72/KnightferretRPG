//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System;
using UnityEngine;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyTransitModule : CozyModule
    {

        [System.Serializable]
        public struct TimeWeightRelation
        {
            [MeridiemTimeAttribute] public float time; [Range(0, 360)] public float sunHeight; [Range(0, 1)] public float weight;

            public TimeWeightRelation(float time, float sunHeight, float weight)
            {
                this.time = time;
                this.sunHeight = sunHeight;
                this.weight = weight;
            }
        }
        [HideInInspector]
        public AnimationCurve sunMovementCurve;

        [Tooltip("Specifies the default weight of the sunrise.")]
        [CozySearchable]
        public TimeWeightRelation sunriseWeight = new TimeWeightRelation(0.25f, 90, 0.2f);
        [Tooltip("Specifies the default weight of the day.")]
        [CozySearchable]
        public TimeWeightRelation dayWeight = new TimeWeightRelation(0.5f, 180, 0.2f);
        [Tooltip("Specifies the default weight of the sunset.")]
        [CozySearchable]
        public TimeWeightRelation sunsetWeight = new TimeWeightRelation(0.75f, 270, 0.2f);
        [Tooltip("Specifies the default weight of the night.")]
        [CozySearchable]
        public TimeWeightRelation nightWeight = new TimeWeightRelation(1, 360, 0.2f);

        [Tooltip("Specifies the day length multiplier in the spring.")]
        [Range(-1, 1)]
        [CozySearchable]
        public float springDayLengthOffset = 0;
        [Tooltip("Specifies the day length multiplier in the summer.")]
        [Range(-1, 1)]
        [CozySearchable]
        public float summerDayLengthOffset = 0.4f;
        [Tooltip("Specifies the day length multiplier in the fall.")]
        [Range(-1, 1)]
        [CozySearchable]
        public float fallDayLengthOffset = 0;
        [Tooltip("Specifies the day length multiplier in the winter.")]
        [Range(-1, 1)]
        [CozySearchable]
        public float winterDayLengthOffset = -0.3f;


        [HideTitle(4)]
        public AnimationCurve dayWeightsDisplayCurve;
        [HideTitle(4)]
        public AnimationCurve yearWeightsCurve;
        public enum TimeCurveSettings { linearDay, simpleCurve, advancedCurve }
        public TimeCurveSettings timeCurveSettings;

        [System.Serializable]
        public class TimeBlock
        {
            public MeridiemTime start;
            public MeridiemTime end;
            public TimeBlock(float startDayPercentage, float endDayPercentage)
            {
                start = startDayPercentage;
                end = endDayPercentage;
            }

        }

        [CozySearchable]
        public TimeBlock dawnBlock = new TimeBlock(4f / 24f, 5.5f / 24f);
        [CozySearchable]
        public TimeBlock morningBlock = new TimeBlock(6f / 24f, 7f / 24f);
        [CozySearchable]
        public TimeBlock dayBlock = new TimeBlock(7.5f / 24f, 9f / 24f);
        [CozySearchable]
        public TimeBlock afternoonBlock = new TimeBlock(13f / 24f, 14f / 24f);
        [CozySearchable]
        public TimeBlock eveningBlock = new TimeBlock(16f / 24f, 18f / 24f);
        [CozySearchable]
        public TimeBlock twilightBlock = new TimeBlock(20f / 24f, 21f / 24f);
        [CozySearchable]
        public TimeBlock nightBlock = new TimeBlock(21f / 24f, 22f / 24f);

        public enum TimeBlockName { dawn, morning, day, afternoon, evening, twilight, night }

        public void GetModifiedDayPercent()
        {

            yearWeightsCurve = new AnimationCurve(new Keyframe[5]
            {
                new Keyframe(0, winterDayLengthOffset, 0, 0),
                new Keyframe(0.25f, springDayLengthOffset, 0, 0),
                new Keyframe(0.5f, summerDayLengthOffset, 0, 0),
                new Keyframe(0.75f, fallDayLengthOffset, 0, 0),
                new Keyframe(1, winterDayLengthOffset, 0, 0)
            });

            float offset = yearWeightsCurve.Evaluate(weatherSphere.timeModule.yearPercentage) / 5;

            switch (timeCurveSettings)
            {

                case TimeCurveSettings.advancedCurve:
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                new Keyframe(sunriseWeight.time - offset, sunriseWeight.sunHeight, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                new Keyframe(dayWeight.time, dayWeight.sunHeight, 0, 0, dayWeight.weight, dayWeight.weight),
                new Keyframe(sunsetWeight.time + offset, sunsetWeight.sunHeight, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                new Keyframe(1, sunsetWeight.sunHeight > dayWeight.sunHeight ? 360 : 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });

                    dayWeightsDisplayCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                new Keyframe(sunriseWeight.time - offset, sunriseWeight.sunHeight, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                new Keyframe(dayWeight.time, dayWeight.sunHeight, 0, 0, dayWeight.weight, dayWeight.weight),
                new Keyframe(sunsetWeight.time + offset, sunsetWeight.sunHeight > 180 ? 360 - sunsetWeight.sunHeight : sunsetWeight.sunHeight, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                new Keyframe(1, 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });
                    break;

                case TimeCurveSettings.simpleCurve:
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                new Keyframe(0.25f - offset, 90f, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                new Keyframe(0.5f, 180f, 0, 0, dayWeight.weight, dayWeight.weight),
                new Keyframe(0.75f + offset, 270f, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                new Keyframe(1, 360, 0, 0, nightWeight.weight, nightWeight.weight)
                    });

                    dayWeightsDisplayCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                new Keyframe(0.25f - offset, 90f, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                new Keyframe(0.5f, 180f, 0, 0, dayWeight.weight, dayWeight.weight),
                new Keyframe(0.75f + offset, 90, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                new Keyframe(1, 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });
                    break;

                case TimeCurveSettings.linearDay:
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, 0, 0),
                new Keyframe(0.25f - offset, 90, 0, 0, 0, 0),
                new Keyframe(0.5f, 180, 0, 0, 0, 0),
                new Keyframe(0.75f + offset, 270, 0, 0, 0, 0),
                new Keyframe(1, 360, 0, 0, 0, 0)
                    });

                    dayWeightsDisplayCurve = new AnimationCurve(new Keyframe[5]
                    {
                new Keyframe(0, 0, 0, 0, 0, 0),
                new Keyframe(0.25f - offset, 90, 0, 0, 0, 0),
                new Keyframe(0.5f, 180, 0, 0, 0, 0),
                new Keyframe(0.75f + offset, 90, 0, 0, 0, 0),
                new Keyframe(1, 0, 0, 0, 0, 0)
                    });
                    break;
            }
        }

        public void GetSunTransitTime(out MeridiemTime sunrise, out MeridiemTime sunset)
        {
            yearWeightsCurve = new AnimationCurve(new Keyframe[5]
            {
                new Keyframe(0, winterDayLengthOffset, 0, 0),
                new Keyframe(0.25f, springDayLengthOffset, 0, 0),
                new Keyframe(0.5f, summerDayLengthOffset, 0, 0),
                new Keyframe(0.75f, fallDayLengthOffset, 0, 0),
                new Keyframe(1, winterDayLengthOffset, 0, 0)
            });

            float offset = yearWeightsCurve.Evaluate(weatherSphere.timeModule.yearPercentage) / 5;
            sunrise = 0.25f - offset;
            sunset = 0.75f + offset;

            if (timeCurveSettings == TimeCurveSettings.advancedCurve)
            {
                sunrise = sunriseWeight.time - offset;
                sunset = sunsetWeight.time + offset;
            }

        }

        public override void InitializeModule()
        {
            base.SetupModule(new Type[1] { typeof(CozyTimeModule) });

            CozyWeather.Events.onNewDay += GetModifiedDayPercent;
            if (weatherSphere.timeModule)
            {
                weatherSphere.timeModule.transit = this;

            }

        }


        void Start()
        {
            SetupTimeEvents();
            GetModifiedDayPercent();
        }

        void Update()
        {
            ManageTimeEvents();
        }

        private void ManageTimeEvents()
        {

            if (weatherSphere.timeModule.currentTime > weatherSphere.events.timeToCheckFor && !(weatherSphere.timeModule.currentTime > nightBlock.start && weatherSphere.events.timeToCheckFor == dawnBlock.start))
            {
                if (weatherSphere.timeModule.currentTime > nightBlock.start && weatherSphere.events.timeToCheckFor == nightBlock.start)
                {
                    weatherSphere.events.RaiseOnNight();
                    weatherSphere.events.timeToCheckFor = dawnBlock.start;
                }
                else if (weatherSphere.timeModule.currentTime > twilightBlock.start && weatherSphere.events.timeToCheckFor == twilightBlock.start)
                {
                    weatherSphere.events.RaiseOnTwilight();
                    weatherSphere.events.timeToCheckFor = nightBlock.start;
                }
                else if (weatherSphere.timeModule.currentTime > eveningBlock.start && weatherSphere.events.timeToCheckFor == eveningBlock.start)
                {
                    weatherSphere.events.RaiseOnEvening();
                    weatherSphere.events.timeToCheckFor = twilightBlock.start;
                }
                else if (weatherSphere.timeModule.currentTime > afternoonBlock.start && weatherSphere.events.timeToCheckFor == afternoonBlock.start)
                {
                    weatherSphere.events.RaiseOnAfternoon();
                    weatherSphere.events.timeToCheckFor = eveningBlock.start;
                }
                else if (weatherSphere.timeModule.currentTime > dayBlock.start && weatherSphere.events.timeToCheckFor == dayBlock.start)
                {
                    weatherSphere.events.RaiseOnDay();
                    weatherSphere.events.timeToCheckFor = afternoonBlock.start;
                }
                else if (weatherSphere.timeModule.currentTime > morningBlock.start && weatherSphere.events.timeToCheckFor == morningBlock.start)
                {
                    weatherSphere.events.RaiseOnMorning();
                    weatherSphere.events.timeToCheckFor = dayBlock.start;
                }
                else
                {
                    weatherSphere.events.RaiseOnDawn();
                    weatherSphere.events.timeToCheckFor = morningBlock.start;
                }
            }

            // if (weatherSphere.timeModule.currentTime < weatherSphere.events.timeToCheckFor - 0.25f) { SetupTimeEvents(); }
            if (Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 24) != weatherSphere.events.currentHour)
            {
                weatherSphere.events.currentHour = Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 24);
                weatherSphere.events.RaiseOnNewHour();
            }
            if (Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 1440) != weatherSphere.events.currentMinute)
            {
                weatherSphere.events.currentMinute = Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 1440);
                weatherSphere.events.RaiseOnMinutePass();
            }

        }

        private void SetupTimeEvents()
        {
            weatherSphere.events.timeToCheckFor = dawnBlock.start;
            if (weatherSphere.timeModule.currentTime > dawnBlock.start)
                weatherSphere.events.timeToCheckFor = morningBlock.start;
            if (weatherSphere.timeModule.currentTime > morningBlock.start)
                weatherSphere.events.timeToCheckFor = dayBlock.start;
            if (weatherSphere.timeModule.currentTime > dayBlock.start)
                weatherSphere.events.timeToCheckFor = afternoonBlock.start;
            if (weatherSphere.timeModule.currentTime > afternoonBlock.start)
                weatherSphere.events.timeToCheckFor = eveningBlock.start;
            if (weatherSphere.timeModule.currentTime > eveningBlock.start)
                weatherSphere.events.timeToCheckFor = twilightBlock.start;
            if (weatherSphere.timeModule.currentTime > twilightBlock.start)
                weatherSphere.events.timeToCheckFor = nightBlock.start;
            if (weatherSphere.timeModule.currentTime > nightBlock.start)
                weatherSphere.events.timeToCheckFor = dawnBlock.start;

            weatherSphere.events.currentHour = Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 24);
            weatherSphere.events.currentMinute = Mathf.FloorToInt(weatherSphere.timeModule.currentTime * 1440);


        }

        public float ModifyDayPercentage(float input)
        {
            return sunMovementCurve.Evaluate(input);
        }

        public TimeBlockName GetTimeBlock()
        {
            TimeBlockName currentBlock = TimeBlockName.night;
            float time = weatherSphere.timeModule.currentTime;

            if (time > dawnBlock.start && time < morningBlock.start)
                currentBlock = TimeBlockName.dawn;
            if (time > morningBlock.start && time < dayBlock.start)
                currentBlock = TimeBlockName.morning;
            if (time > dayBlock.start && time < afternoonBlock.start)
                currentBlock = TimeBlockName.day;
            if (time > afternoonBlock.start && time < eveningBlock.start)
                currentBlock = TimeBlockName.afternoon;
            if (time > eveningBlock.start && time < twilightBlock.start)
                currentBlock = TimeBlockName.evening;
            if (time > twilightBlock.start && time < nightBlock.start)
                currentBlock = TimeBlockName.twilight;

            return currentBlock;

        }

        public TimeBlockName GetTimeBlock(float time)
        {
            TimeBlockName currentBlock = TimeBlockName.night;

            if (time > dawnBlock.start && time < morningBlock.start)
                currentBlock = TimeBlockName.dawn;
            if (time > morningBlock.start && time < dayBlock.start)
                currentBlock = TimeBlockName.morning;
            if (time > dayBlock.start && time < afternoonBlock.start)
                currentBlock = TimeBlockName.day;
            if (time > afternoonBlock.start && time < eveningBlock.start)
                currentBlock = TimeBlockName.afternoon;
            if (time > eveningBlock.start && time < twilightBlock.start)
                currentBlock = TimeBlockName.evening;
            if (time > twilightBlock.start && time < nightBlock.start)
                currentBlock = TimeBlockName.twilight;

            return currentBlock;

        }



    }


}