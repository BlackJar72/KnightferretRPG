using UnityEngine;

namespace kfutils {
public class ClockTurner : MonoBehaviour
{

        [Tooltip("This must be provided.  The hour hand of the clock.")]
        [SerializeField] GameObject hourHand;
        [Tooltip("This must be provided (use an empty game object if you don't want a minute hand).  The minute hand of the clock.")]
        [SerializeField] GameObject minuteHand;
        [Tooltip("This is the value that will be used to scale the time; 60 gives a 24 minute day (i.e.,one real world minute = one in game hour).")]
        [SerializeField] float timeScale = 60;
        [Tooltip("This is the hour the clock will start at, and should be the hour at the start of the game; not used if pre-processing hours.")]
        [SerializeField] float startOffsetHours = 0;
        [Tooltip("This is an implementation of ATimeForClock that provides the time to the clock.  If null, Unity's time will be used.")]
        [SerializeField] ATimeForClock timeProvider;
        [Tooltip("If true, the time provider will be expected to return the time of day in hours; if false, it will be expected to return the time in seconds from the start of the game.")]
        [SerializeField] bool preProcessedHours = true;

        [Tooltip("Leave as null (none) if the clock has no pendulum.")]
        [SerializeField] GameObject pendulum;
        [SerializeField] float maxPendulumSwing = 10;
        private float startOffset;

        private const float realSecondsPerRotation = 60 * 60 * 24;
        private float secondsPerRotation;
        private float secondsPerDegree;

        private delegate void MoveHands();
        private MoveHands moveHands;


        void Awake()
        {
            if(timeScale == 0)
            {
                timeScale = 1;
            }
            secondsPerRotation = realSecondsPerRotation / timeScale;
            secondsPerDegree = secondsPerRotation / 360;
            startOffset = (startOffsetHours * secondsPerRotation) / 12;
        }


        // Start is called before the first frame update
        void Start()
        {
            if(pendulum == null) {
                if(timeProvider == null) {
                    moveHands = MoveHandsByUnityTime;        
                } else if(preProcessedHours) {
                    moveHands = MoveHandsByScaledTime;
                } else {
                    moveHands = MoveHandsByRawTime;
                }
            } else {
                if(timeProvider == null) {
                    moveHands = MoveHandsByUnityTimePendulum;
                } else if(preProcessedHours) {
                    moveHands = MoveHandsByScaledTimePendulum;
                } else {
                    moveHands = MoveHandsByRawTimePendulum;
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            moveHands();
            if(pendulum != null) {
                pendulum.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time % 360) * 10);
            }
        }


        private void MoveHandsByUnityTime() {
            float hourHandAngle = (Time.time + startOffset) / secondsPerDegree;
            float minuteHandAngle = hourHandAngle * 12;
            hourHand.transform.localRotation = Quaternion.Euler(0, 0, hourHandAngle);
            minuteHand.transform.localRotation = Quaternion.Euler(0, 0, minuteHandAngle);
        }


        private void MoveHandsByRawTime() {
            float hourHandAngle = (timeProvider.GetTime() + startOffset) / secondsPerDegree;
            float minuteHandAngle = hourHandAngle * 12;
            hourHand.transform.localRotation = Quaternion.Euler(0, 0, hourHandAngle);
            minuteHand.transform.localRotation = Quaternion.Euler(0, 0, minuteHandAngle);
        }


        private void MoveHandsByScaledTime() {
            float hourHandAngle = timeProvider.GetTime() / 30;
            float minuteHandAngle = hourHandAngle * 12;
            hourHand.transform.localRotation = Quaternion.Euler(0, 0, hourHandAngle);
            minuteHand.transform.localRotation = Quaternion.Euler(0, 0, minuteHandAngle);
        }


        private void MoveHandsByUnityTimePendulum() {
            MoveHandsByUnityTime();
            pendulum.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time % 360) * maxPendulumSwing);
        }


        private void MoveHandsByRawTimePendulum() {
            MoveHandsByRawTime();
            pendulum.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time % 360) * maxPendulumSwing);
        }


        private void MoveHandsByScaledTimePendulum() {
            MoveHandsByScaledTime();
            pendulum.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time % 360) * maxPendulumSwing);
        }


        /// <summary>
        /// A base class for providing time to the clock, derived from scriptable object.
        /// 
        /// For use with raw time, override GetTime() to return the time in seconds. This 
        /// should be the time since the game began (from the very beginning, saved in in 
        /// save files, etc).  Time off-set and scaling are not included.
        /// 
        /// For use with pre-processed time, override GetTime() to return the time in hours. 
        /// This needs to include any time off-set, as none is provided; it is assumed your 
        /// are feeding the clock the actual in-game hour.
        /// 
        /// To use, create a new scriptable object that inherits from this class, and assign
        /// it to the ClockTurner component in the inspector.  If you are just using Unity's
        /// time, you do not need to assign a time provider.
        /// </summary>
        public abstract class ATimeForClock : ScriptableObject {
            /// <summary>
            /// Should return either time in seconds since the beginning of the game for raw 
            //  time, or current time of day for pre-processed time.
            /// </summary>
            /// <returns></returns>
            public abstract float GetTime();
        }



    }

}
