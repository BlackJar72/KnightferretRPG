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

        public EActivityRun ActivityCode => throw new System.NotImplementedException();

        public float DesireabilityFactor => throw new System.NotImplementedException();

        public ActivityHelper.EEndCondition EndCondition => throw new System.NotImplementedException();

        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }


        public void RunSpecialCode(ITalkerAI ai, AIState aiState)
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldEndActivity()
        {
            throw new System.NotImplementedException();
        }

        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }
    }



}
