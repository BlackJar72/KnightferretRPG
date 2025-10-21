using UnityEngine;


namespace kfutils.rpg
{

    // QUESTION: Should this be its own monobehavior, or should ITalker or ITalkerAI inherit IActivity Object???

    public class ActivitySocial : MonoBehaviour, IActivityObject
    {

        public AbstractAction UseAction => throw new System.NotImplementedException();
        public float TimeToDo => throw new System.NotImplementedException();

        public ENeeds GetNeed => throw new System.NotImplementedException();

        public EObjectActivity ActivityType => throw new System.NotImplementedException();

        public float Satisfaction => throw new System.NotImplementedException();

        public float DesireabilityFactor => throw new System.NotImplementedException();

        public ActivityHelper.EEndCondition EndCondition => throw new System.NotImplementedException();

        public ActivityHelper.ECodeToRun StartCode => throw new System.NotImplementedException();

        public ActivityHelper.ECodeToRun ContinuousCode => throw new System.NotImplementedException();

        public ActivityHelper.ECodeToRun EndCode => throw new System.NotImplementedException();

        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }

        public void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }

        public void RunEndCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }

        public void RunStartCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, this, aiState);
        }

        
    }



}
