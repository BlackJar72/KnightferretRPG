using UnityEngine;
using static kfutils.rpg.WorldTime;


namespace kfutils.rpg {

    [System.Serializable]
    public class HumanTime
    {
        [SerializeField][Range (0, 59)] int second;
        [SerializeField][Range (0, 59)] int minute;
        [SerializeField][Range (0, 23)] int hour;
        [SerializeField][Range (0, 27)] int day;
        [SerializeField] int month;

        private float fraction;

        public int Second => second;
        public int Minute => minute;
        public int Hour => hour;
        public int Day => day;
        public int Month => month;
        public float Fraction => fraction;


        public HumanTime()
        {
            month = day = hour = minute = second = 0;
        }


        public HumanTime(int second, int minute, int hour, int day, int month)
        {
            this.second = second;
            this.minute = minute;
            this.hour = hour;
            this.day = day;
            this.month = month;
        }


        public HumanTime(double time)
        {
            Set(time);
        }


        public void Set(int second, int minute, int hour, int day, int month)
        {
            this.second = second;
            this.minute = minute;
            this.hour = hour;
            this.day = day;
            this.month = month;
        }


        public void Set(double time)
        {
            month = (int)(time / MONTH);
            time %= MONTH;
            day = (int)(time / DAY);
            time %= DAY;
            hour = (int)(time / HOUR);
            time %= HOUR;
            minute = (int)(time / MINUTE);
            second = (int)time;
            fraction = (float)(time - second);
        }


        public GameTime ToGameTime()
        {
            return new(second + (minute * MINUTE) + (hour * HOUR) + (day * DAY) + (month * MONTH) + fraction);
        }
    }


    public readonly struct GameTime
    {
        private readonly double seconds;

        public double time => seconds;
        public float ftime => (float)seconds;
        public int Day => Mathf.FloorToInt((float)(seconds / RT_DAY));
        public int Week => Mathf.FloorToInt((float)(seconds / RT_WEEK));
        public int Month => Mathf.FloorToInt((float)(seconds / RT_MONTH));
        public int DayOfWeek => Day % 7;
        public float TimeInDay => (float)(seconds / RT_DAY) - Day;
        public int DayOfMonth => Day % 28;
        public float TimeInMonth => (float)(seconds / RT_MONTH) - Month; 

        public static implicit operator double(GameTime t) => t.seconds;
        public static implicit operator GameTime(double t) => new(t);
        public static explicit operator float(GameTime t) => (float)t.seconds;
        public static implicit operator GameTime(float t) => new(t);
        public static explicit operator HumanTime(GameTime t) => new(t.seconds);
        public static implicit operator GameTime(HumanTime t) => new(t.ToGameTime());

        public GameTime(double seconds) => this.seconds = seconds;
    }

}

