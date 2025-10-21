using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class ActivityItem : IActivityObject
    {

        // QUESTION: Shouod this exist, or be combined with ItemPrototype???

        [SerializeField] ENeeds theNeeds;
        [SerializeField] EObjectActivity activityType;
        [Range(0.0f, 1.0f)][SerializeField] float satisfaction;
        [Range(0.0f, 2.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] AbstractAction useAction;
        [SerializeField] ActivityHelper.EEndCondition endCondition;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunAtStart;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunContinuously;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunAtEnd;
  

        public ENeeds TheNeed => theNeeds;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public AbstractAction UseAction => useAction;
        public ENeeds GetNeed => theNeeds;
        public float Satisfaction => satisfaction;
        public EObjectActivity ActivityType => activityType;
        public ActivityHelper.ECodeToRun StartCode => codeToRunAtStart;
        public ActivityHelper.ECodeToRun ContinuousCode => codeToRunContinuously;
        public ActivityHelper.ECodeToRun EndCode => codeToRunAtEnd;
        public ActivityHelper.EEndCondition EndCondition => endCondition;
        public delegate void SpecialCode(ITalkerAI ai, ActivityItem activity, AIState aiState);


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public virtual float GetUtility(ITalkerAI entity)
        {
            float desireability = 0.0f;
            if ((theNeeds & ENeeds.ENERGY) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.ENERGY));
            if ((theNeeds & ENeeds.FOOD) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.FOOD));
            if ((theNeeds & ENeeds.SOCIAL) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.SOCIAL));
            if ((theNeeds & ENeeds.ENJOYMENT) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.ENJOYMENT));
            return desireability;
        }


        public void RunStartCode(ITalkerAI ai, NeedSeekState aiState)
        {
            ActivityHelper.RunStartCode(ai, this, aiState);
        }


        public void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState)
        {
            ActivityHelper.RunContinuousCode(ai, this, aiState);
        }


        public void RunEndCode(ITalkerAI ai, NeedSeekState aiState)
        {
            ActivityHelper.RunEndCode(ai, this, aiState);
        }


        private float GetUtilityForNeed(ITalkerAI entity, ENeedID theNeed)
        {
            float desireability = (satisfaction * desireabilityFactor)
                                + Mathf.Sqrt((satisfaction * desireabilityFactor) / (timeToDo + 60f) + 1) - 1;
            desireability *= entity.GetNeed(theNeed).GetDrive();
            return desireability;
        }


        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, this, aiState);
        }








    }



}
