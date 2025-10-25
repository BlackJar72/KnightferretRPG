using UnityEngine;


namespace kfutils.rpg
{

    public class WorldTime : MonoBehaviour
    {
        public const double MINUTE = 60.0;
        public const double HOUR = MINUTE * 60.0f;
        public const double DAY = HOUR * 24.0;
        public const double WEEK = DAY * 7.0;


        private static WorldTime instance;
        private static double seconds;


        public static double time => seconds;
        public static float ftime => (float)seconds;

#if UNITY_EDITOR
        private static long frame = 0;
        public static long Frame => frame;
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
        }


        void OnDestroy()
        {
            instance = null;
        }


        void Update()
        {
            seconds += Time.deltaTime;
#if UNITY_EDITOR
            frame++;
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