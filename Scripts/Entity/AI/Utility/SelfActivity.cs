using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Used to represent activities done by a character without use of any particualar 
    /// object, item, etc., but only involving themself.  E.g., idling or wandering.
    /// </summary>
    [System.Serializable]
    public class SelfActivity : IActivityObject
    {

        [SerializeField] AbstractAction useAction;
        [SerializeField] float timeToDo;
        [SerializeField] EActivityRun activityCode;
        [SerializeField] ECodeToRun codeToRun;

        public AbstractAction UseAction => useAction;
        public float Satisfaction => 0.0f;
        public float TimeToDo => timeToDo;
        public ENeeds GetNeed => ENeeds.NONE;
        public EObjectActivity ActivityType => EObjectActivity.SELF;
        public EActivityRun ActivityCode => activityCode;

        public delegate void SpecialCode(ITalkerAI ai, SelfActivity activity, AIState aiState);


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            return (Random.value * 0.5f) + 0.1f; // This will likely need to be tweaked
        }


        public void RunSpecialCode(ITalkerAI ai, AIState aiState)
        {
            effects[(int)codeToRun](ai, this, aiState);
        }


        #region Special Code
        /*******************************************************************************************/
        /*                                 SPECIAL CODE                                            */
        /*******************************************************************************************/


        public enum ECodeToRun
        {
            NONE = 0,
            WANDER = 1
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        private static SpecialCode[] effects = new SpecialCode[]{
            DoNothing,
            Wander

        };


        private static void DoNothing(ITalkerAI ai, SelfActivity activity, AIState aiState) { }


        private static void Wander(ITalkerAI ai, SelfActivity activity, AIState aiState)
        {
            // TODO (FIXME): A better way to pick location to wander to; this ignores buildings / boundaries, 
            // presence or absense of navemesh, general validity of destination, and height.  It should work 
            // for testing in the test village.  Also, there should be some kind of anchor to keep from wandering 
            // too far off. 
            float distance = Random.Range(2.0f, 8.0f);
            float direction = Random.Range(0.0f, 360.0f);
            Vector3 vector = new Vector3(distance * Mathf.Sin(direction), 0.0f, distance * Mathf.Cos(direction));
            Vector3 destination = ai.GetTransform.position + vector;
            ai.SetDestination(destination, Random.value * 0.5f + 0.25f);
        }





        #endregion

    }


}
