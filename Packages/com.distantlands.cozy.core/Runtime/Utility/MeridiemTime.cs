using System;
using UnityEngine;


namespace DistantLands.Cozy
{
    [Serializable]
    public class MeridiemTime
    {

        public int hours;
        public int minutes;
        public int seconds;
        public int milliseconds;


        public float timeAsPercentage;

        public MeridiemTime() { }

        public MeridiemTime(int hour, int minute)
        {
            this.hours = hour;
            minutes = minute;
            // timeAsPercentage = (hour * 3600000f + minute * 60000f) / 86400000f;
        }
        public MeridiemTime(int hour, int minute, int second, int millisecond)
        {
            hours = hour;
            minutes = minute;
            seconds = second;
            milliseconds = millisecond;
            // timeAsPercentage = (hour * 3600000f + minute * 60000f + second * 1000f + millisecond) / 86400000f;
        }
        public static implicit operator MeridiemTime(float floatValue)
        {
            MeridiemTime time = new MeridiemTime();
            time.hours = Mathf.FloorToInt(floatValue * 24f);
            time.minutes = Mathf.FloorToInt(floatValue * 1440f % 60f);
            time.seconds = Mathf.FloorToInt(floatValue * 86400f % 60f);
            time.milliseconds = Mathf.FloorToInt(floatValue * 86400000f % 1000f);
            return time;
        }
        public static implicit operator float(MeridiemTime time) => (time.hours * 3600000f + time.minutes * 60000f + time.seconds * 1000f + time.milliseconds) / 86400000f;
        public static implicit operator DateTime(MeridiemTime time) => new DateTime(1, 1, 1, time.hours, time.minutes, time.seconds, time.milliseconds);
        public static implicit operator string(MeridiemTime time) => $"{time.hours:D2}:{time.minutes:D2}";
        public new string ToString() => $"{hours:D2}:{minutes:D2}";
        public string FullString() => $"{hours:D2}:{minutes:D2}:{seconds:D2}:{milliseconds:D4}";
    }
}