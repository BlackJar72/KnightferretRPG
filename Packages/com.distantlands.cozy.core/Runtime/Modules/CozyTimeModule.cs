//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections;
using UnityEngine;
using DistantLands.Cozy.Data;
using UnityEngine.Serialization;
using System;

namespace DistantLands.Cozy
{
    [ExecuteAlways]
    public class CozyTimeModule : CozyModule
    {

        public CozyTransitModule transit;
        public PerennialProfile perennialProfile;
        public CozyDateOverride overrideDate;
        [Range(0, 1)]
        public float yearPercentage = 0;
        public float modifiedDayPercentage
        {
            get
            {
                return transit ? transit.ModifyDayPercentage(currentTime) / 360 : currentTime;
            }
        }
        public bool transitioningTime;

        [FormerlySerializedAs("m_DayPercentage")]
        [CozySearchable]
        public MeridiemTime currentTime = 0;

        public int AbsoluteDay => currentDay + DaysPerYear * currentYear;

        [CozySearchable]
        public int currentDay;
        [CozySearchable]
        public int currentYear;
        public CozyTimeModule parentModule;


        public override void InitializeModule()
        {
            base.InitializeModule();
            weatherSphere.timeModule = this;
        }

        internal override bool CheckIfModuleCanBeRemoved(out string warning)
        {
            if (weatherSphere.GetModule<CozyTransitModule>() != null)
            {
                warning = "Transit Module";
                return false;
            }
            warning = "";
            return true;
        }

        internal override bool CheckIfModuleCanBeAdded(out string warning)
        {
            if (weatherSphere.GetModule<SystemTimeModule>() != null)
            {
                warning = "System Time Module";
                return false;
            }
            warning = "";
            return true;
        }

        void Start()
        {
            SetupTime();
        }

        void Update()
        {

            if (weatherSphere.timeModule == null)
                weatherSphere.timeModule = this;

            ManageTime();

            yearPercentage = GetCurrentYearPercentage();

        }

        void SetupTime()
        {
            if (perennialProfile.resetTimeOnStart)
                currentTime = perennialProfile.startTime;


            if (perennialProfile.realisticYear)
                perennialProfile.daysPerYear = perennialProfile.GetRealisticDaysPerYear(currentYear);

        }

        /// <summary>
        /// Constrains the time to fit within the length parameters set on the perennial profile.
        /// </summary> 
        private void ConstrainTime()
        {
            if (currentTime >= 1)
            {
                currentTime -= 1;
                ChangeDay(1);
                weatherSphere.events.RaiseOnDayChange();
            }

            if (currentTime < 0)
            {
                currentTime += 1;
                ChangeDay(-1);
                weatherSphere.events.RaiseOnDayChange();
            }
        }

        private void ChangeDay(int change)
        {

            if (overrideDate)
            {
                overrideDate.ChangeDay(change);
                return;
            }

            if (!perennialProfile.progressDay)
                return;

            currentDay += change;

            if (currentDay >= perennialProfile.daysPerYear)
            {
                currentDay -= perennialProfile.daysPerYear;
                currentYear++;
                weatherSphere.events.RaiseOnYearChange();
            }

            if (currentDay < 0)
            {
                currentDay += perennialProfile.daysPerYear;
                currentYear--;
                weatherSphere.events.RaiseOnYearChange();
            }
        }

        [Obsolete("GetDaysPerYear() is deprecated. Please use DaysPerYear instead.")]
        public int GetDaysPerYear()
        {
            if (overrideDate)
                return overrideDate.DaysPerYear();

            if (perennialProfile.realisticYear)
                return perennialProfile.GetRealisticDaysPerYear(currentYear);
            else
                return perennialProfile.daysPerYear;
        }

        public int DaysPerYear
        {
            get
            {
                if (overrideDate)
                    return overrideDate.DaysPerYear();

                if (perennialProfile.realisticYear)
                    return perennialProfile.GetRealisticDaysPerYear(currentYear);
                else
                    return perennialProfile.daysPerYear;
            }
        }

        public void GetSunTransitTime(out MeridiemTime sunrise, out MeridiemTime sunset)
        {
            if (transit)
            {
                transit.GetSunTransitTime(out sunrise, out sunset);
                return;
            }

            sunrise = 0.25f;
            sunset = 0.75f;

        }

        /// <summary>
        /// Returns the current year percentage (0 - 1).
        /// </summary> 
        public float GetCurrentYearPercentage()
        {

            if (overrideDate)
                return overrideDate.GetCurrentYearPercentage();

            float dat = DayAndTime();
            return dat / (float)DaysPerYear;
        }

        /// <summary>
        /// Returns the current year percentage (0 - 1) after a number of ticks has passed.
        /// </summary> 
        public float GetCurrentYearPercentage(float inTIme)
        {
            if (overrideDate)
                return overrideDate.GetCurrentYearPercentage(inTIme);

            float dat = DayAndTime() + inTIme;
            return dat / perennialProfile.daysPerYear;
        }

        /// <summary>
        /// Gets the current day plus the current day percentage (0-1). 
        /// </summary> 
        public float DayAndTime()
        {
            if (overrideDate)
                return overrideDate.DayAndTime();

            return currentDay + currentTime;

        }

        /// <summary>
        /// Manages the movement of time in the scene.
        /// </summary> 
        public void ManageTime()
        {

            if (Application.isPlaying && !perennialProfile.pauseTime)
                currentTime += modifiedTimeSpeed * Time.deltaTime;

            ConstrainTime();

        }

        public float modifiedTimeSpeed
        {
            get
            {
                return perennialProfile.timeMovementSpeed * (perennialProfile.pauseTime ? 0 : 1) * (perennialProfile.modulateTimeSpeed ? perennialProfile.timeSpeedMultiplier.Evaluate(currentTime) : 1) / 1440;
            }
        }

        /// <summary>
        /// Skips the weather system forward by the ticksToSkip value.
        /// </summary> 
        public void SkipTime(MeridiemTime timeToSkip)
        {


            currentTime += (float)timeToSkip;

            if (weatherSphere.GetModule<CozyAmbienceModule>())
                weatherSphere.GetModule<CozyAmbienceModule>().SkipTime(timeToSkip);

            foreach (CozySystem i in weatherSphere.systems)
            {
                i.SkipTime(timeToSkip);
            }

        }

        public void SkipTime(MeridiemTime timeToSkip, int daysToSkip)
        {

            currentTime += (float)timeToSkip;
            currentDay += daysToSkip;

            if (weatherSphere.GetModule<CozyAmbienceModule>())
                weatherSphere.GetModule<CozyAmbienceModule>().SkipTime(timeToSkip + daysToSkip);

            foreach (CozySystem i in weatherSphere.systems)
            {
                i.SkipTime(timeToSkip + daysToSkip);
            }
        }

        public void SetHour(int hour)
        {
            currentTime = new MeridiemTime(hour, currentTime.minutes, currentTime.seconds, currentTime.milliseconds);
        }
        public void SetMinute(int minute)
        {
            currentTime = new MeridiemTime(currentTime.hours, minute, currentTime.seconds, currentTime.milliseconds);
        }

        /// <summary>
        /// Returns the title for the current month.
        /// </summary> 
        public string MonthTitle(float month)
        {


            if (perennialProfile.realisticYear)
            {

                GetCurrentMonth(out string monthName, out int monthDay, out float monthPercentage);
                return monthName + " " + monthDay;

            }
            else
            {

                float j = Mathf.Floor(month * 12);
                float monthLength = perennialProfile.daysPerYear / 12;
                float monthTime = DayAndTime() - (j * monthLength);

                PerennialProfile.DefaultYear monthName = (PerennialProfile.DefaultYear)j;
                PerennialProfile.TimeDivisors monthTimeName = PerennialProfile.TimeDivisors.Mid;

                if ((monthTime / monthLength) < 0.33f)
                    monthTimeName = PerennialProfile.TimeDivisors.Early;
                else if ((monthTime / monthLength) > 0.66f)
                    monthTimeName = PerennialProfile.TimeDivisors.Late;
                else
                    monthTimeName = PerennialProfile.TimeDivisors.Mid;


                return $"{monthTimeName} {monthName}";
            }
        }

        public void GetCurrentMonth(out string monthName, out int monthDay, out float monthPercentage)
        {

            int i = currentDay;
            int j = 0;

            while (i > ((perennialProfile.useLeapYear && currentYear % 4 == 0) ? perennialProfile.leapYear[j].days : perennialProfile.standardYear[j].days))
            {

                i -= (perennialProfile.useLeapYear && currentYear % 4 == 0) ? perennialProfile.leapYear[j].days : perennialProfile.standardYear[j].days;

                j++;

                if (j >= ((perennialProfile.useLeapYear && currentYear % 4 == 0) ? perennialProfile.leapYear.Length : perennialProfile.standardYear.Length))
                    break;

            }

            PerennialProfile.Month k = (perennialProfile.useLeapYear && currentYear % 4 == 0) ? perennialProfile.leapYear[j] : perennialProfile.standardYear[j];

            monthName = k.name;
            monthDay = i;
            monthPercentage = k.days;


        }


        /// <summary>
        /// Smoothly skips a set amount of time into the future.
        /// <param name="timeToSkip">The day percentage (given in a float or Meridiem Time) to skip forward.</param>
        /// <param name="time">The time in seconds it takes to transition to the new time.</param>
        /// </summary> 
        public void TransitionTime(float timeToSkip, float time)
        {

            StartCoroutine(TransitionTime(currentTime, timeToSkip, time));

        }

        IEnumerator TransitionTime(float startDayPercentage, float timeToSkip, float time)
        {

            transitioningTime = true;
            float t = time;
            float targetTime = timeToSkip % 1;
            float targetDay = Mathf.Floor(timeToSkip);
            float transitionSpeed = timeToSkip / time;

            while (t > 0)
            {

                float div = 1 - (t / time);
                yield return new WaitForEndOfFrame();

                currentTime += Time.deltaTime * transitionSpeed;

                t -= Time.deltaTime;

            }

            transitioningTime = false;

        }
    }

}