using UnityEngine;


namespace kfutils.rpg {

    public class WorldTime : MonoBehaviour
    {
        public const float TIME_SCALE = 60f; // How many times faster time runs in game; 60 gives a 24 minute day

        // Time Units in In-Game World Time
        public const double MINUTE = 60.0;
        public const double HOUR = MINUTE * 60.0f;
        public const double DAY = HOUR * 24.0;
        public const double WEEK = DAY * 7.0;
        public const double MONTH = WEEK * 4.0;

        // In-Game Time Units in Real Time
        public const double RT_MINUTE = MINUTE / TIME_SCALE;
        public const double RT_HOUR = HOUR / TIME_SCALE;
        public const double RT_DAY = DAY / TIME_SCALE;
        public const double RT_WEEK = WEEK / TIME_SCALE;
        public const double RT_MONTH = MONTH / TIME_SCALE;


        private static WorldTime instance;
        private static double seconds;


        public static double time => seconds;
        public static float ftime => (float)seconds;

        public static int Day => Mathf.FloorToInt((float)(seconds / RT_DAY));
        public static int Week => Mathf.FloorToInt((float)(seconds / RT_WEEK));
        public static int Month => Mathf.FloorToInt((float)(seconds / RT_MONTH));
        public static int DayOfWeek => Day % 7;
        public static float TimeInDay => (float)(seconds / RT_DAY) - Day;
        public static int DayOfMonth => Day % 28;
        public static float TimeInMonth => (float)(seconds / RT_MONTH) - Month;


#if UNITY_EDITOR
        [SerializeField] double worldTime;
        private static long frame = 0;
        public static long Frame => frame;

        [SerializeField] float unityTime;
        [SerializeField] float minute;
        [SerializeField] float hour;
        [SerializeField] float day;
        [SerializeField] float timeInDay;
#endif


        void Awake()
        {
            // Even though member are their own static values, this must be a singleton to avoid multiple calls to Update(), 
            // which would speed up time by a factor of the number of instances.  I'm going hardcore and deleting extra 
            // gameobjects.
            if ((instance != null) && (instance != this) && (instance.gameObject != null) && (instance.gameObject != this))
            {
                // This could be added the the first if, but this is more readable and this should not be called except at 
                // the beginning of the game.
                if ((GetComponent<GameManager>() != null) || (instance.gameObject.GetComponent<GameManager>() == null))
                    Destroy(instance.gameObject);
            }
            instance = this;
            Debug.Log("Minute = " + MINUTE + " => " + RT_MINUTE);
            Debug.Log("Hour = " + HOUR + " => " + RT_HOUR);
            Debug.Log("Day = " + DAY + " => " + RT_DAY);
            Debug.Log("Week = " + WEEK + " => " + RT_WEEK);
        }


        void OnDestroy()
        {
            instance = null;
        }


        void Update()
        {
            seconds += Time.deltaTime;
#if UNITY_EDITOR
            worldTime = seconds;
            frame++;

            unityTime = Time.time;
            minute = (float)(seconds / RT_MINUTE);
            hour = (float)(seconds / RT_HOUR);
            day = (float)(seconds / RT_DAY);
            timeInDay = TimeInDay;
#endif
        }


        public static void SetTime(double t)
        {
            seconds = t;
        }


        public static void NewGame()
        {
            seconds = 0.0;
        }




    }


}