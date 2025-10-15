using UnityEngine;


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
            ANIM_END = 3,
            NOTIFIED = 4
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        public static ShouldEnd[] endDeterminers = new ShouldEnd[]{
            TimedEnd,
            FilledEnd,
            AnimEnded,
            EndNotified
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





        #endregion





    }


}
