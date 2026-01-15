using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DistantLands.Cozy
{
    public class CZY_UIManager : MonoBehaviour
    {
        CozyWeather weatherSphere;
        public Image fuzzyTimeIcon;
        public Sprite sunriseIcon;
        public Sprite sunsetIcon;
        public Sprite dayIcon;
        public Sprite nightIcon;

        public TMP_Text currentTime;
        public TMP_Text currentDate;
        public TMP_Text currentWeather;
        public TMP_Text currentTemp;

        // Start is called before the first frame update
        void Start()
        {

            weatherSphere = CozyWeather.instance;

        }

        // Update is called once per frame
        void Update()
        {

            if (weatherSphere.dayPercentage < new MeridiemTime(5, 30))
                fuzzyTimeIcon.sprite = nightIcon;
            else if (weatherSphere.dayPercentage < new MeridiemTime(9, 30))
                fuzzyTimeIcon.sprite = sunriseIcon;
            else if (weatherSphere.dayPercentage < new MeridiemTime(17, 00))
                fuzzyTimeIcon.sprite = dayIcon;
            else if (weatherSphere.dayPercentage < new MeridiemTime(21, 00))
                fuzzyTimeIcon.sprite = sunsetIcon;
            else
                fuzzyTimeIcon.sprite = nightIcon;

            currentTime.text = $"{(weatherSphere.timeModule.currentTime.hours % 12 == 0 ? 12 : weatherSphere.timeModule.currentTime.hours % 12):D2}:{weatherSphere.timeModule.currentTime.minutes:D2} {(weatherSphere.timeModule.currentTime.hours > 12 ? "PM" : "AM")}";
            currentWeather.text = weatherSphere.weatherModule.ecosystem.currentWeather.name;

            weatherSphere.timeModule.GetCurrentMonth(out string monthName, out int monthDay, out float monthPercentage);
            currentDate.text = $"{monthName.Substring(0, 3)} {monthDay}";
            
            currentTemp.text = $"{Mathf.Floor(weatherSphere.climateModule.currentTemperature)}° F";


        }
    }
}