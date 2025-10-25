using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg
{


    public static class ActivityHelper
    {
        

        #region End Conditions
        /*******************************************************************************************/
        /*                                 End Conditions                                          */
        /*******************************************************************************************/
        public delegate bool ShouldEnd(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState);


        public static bool ShouldEndActivity(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            return endDeterminers[(int)activity.EndCondition](ai, activity, aiState); 
        }


        public enum EEndCondition
        {
            TIMED = 0,
            FILLED = 1,
            ANIM_END = 2,
            NOTIFIED = 3,
            AT_DESTINATION = 4,
            GROUP_FILLED = 5
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        public static ShouldEnd[] endDeterminers = new ShouldEnd[]{
            TimedEnd,
            FilledEnd,
            AnimEnded,
            EndNotified,
            AtDestination,
            GroupFilled
        };


        private static bool TimedEnd(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            return Time.time > aiState.ActivityTimer;
        }


        private static bool FilledEnd(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {            
            return ai.GetNeeds.AreFull(activity.GetNeed) || ai.GetNeeds.AreLow(~activity.GetNeed);
        }


        private static bool AnimEnded(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {            
            return aiState.AnimEnded;
        }


        private static bool EndNotified(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {            
            return aiState.Notified;
        }


        private static bool AtDestination(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            return ai.AtDestination();
        }


        private static bool GroupFilled(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            if ((activity is ActivitySlot slot) && slot.Parent.Completed) return true;
            else if ((activity is GroupActivity group) && group.Completed) return true;
            return ai.GetNeeds.AreFull(activity.GetNeed) || ai.GetNeeds.AreLow(~activity.GetNeed);
        }





        #endregion




        #region Special Code
        /*******************************************************************************************/
        /*                                 SPECIAL CODE                                            */
        /*******************************************************************************************/


        public delegate void SpecialCode(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState);


        public static void RunStartCode(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            effects[(int)activity.StartCode](ai, activity, aiState);
        }


        public static void RunContinuousCode(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            effects[(int)activity.ContinuousCode](ai, activity, aiState);
        }


        public static void RunEndCode(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState)
        {
            effects[(int)activity.EndCode](ai, activity, aiState);
        }




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


        private static void DoNothing(ITalkerAI ai, IActivityObject activity, NeedSeekState aiState) { }


        private static void Wander(ITalkerAI ai, IActivityObject activity, NeedSeekState aiStatee)
        {
            // TODO (FIXME): A better way to pick location to wander to; this ignores buildings / boundaries, 
            // presence or absense of navemesh, general validity of destination, and height.  It should work 
            // for testing in the test village.  Also, there should be some kind of anchor to keep from wandering 
            // too far off. 
            Vector3 destination = ai.GetTransform.position;
            for (int i = 0; i < 10; i++)
            {
                float distance = Random.Range(2.0f, 8.0f);
                float direction = Random.Range(0.0f, 360.0f);
                Vector3 vector = new Vector3(distance * Mathf.Sin(direction), 0.0f, distance * Mathf.Cos(direction));
                destination = ai.GetTransform.position + vector;
                NavMesh.SamplePosition(destination, out NavMeshHit navMeshHit, 3.0f, 1);
                if (navMeshHit.hit)
                {
                    destination = navMeshHit.position;
                    i = 11;
                }
            }
            ai.SetDestination(destination);
        }





        #endregion





    }


}
