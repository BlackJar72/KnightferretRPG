using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Used to represent activities done by a character without use of any particualar 
    /// object, item, etc., but only involving themself.  E.g., idling or wandering.
    /// </summary>
    // [System.Serializable]
    [CreateAssetMenu(menuName = "KF-RPG/AI/Activities/Simple Self Activity", fileName = "SelfActivity", order = 10)]
    public class SelfActivity : ScriptableObject, IActivityObject
    {

        [SerializeField] AbstractAction useAction;
        [SerializeField] float timeToDo;
        [Range(0.0f, 2.0f)][SerializeField] protected float desireabilityFactor = 1.0f;

        [SerializeField] ActivityHelper.EEndCondition endCondition;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunAtStart;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunContinuously;
        [SerializeField] ActivityHelper.ECodeToRun codeToRunAtEnd;

        public AbstractAction UseAction => useAction;
        public float Satisfaction => 0.0f;
        public float TimeToDo => timeToDo;
        public ENeeds GetNeed => ENeeds.NONE;
        public EObjectActivity ActivityType => EObjectActivity.SELF;
        public float DesireabilityFactor => desireabilityFactor;

        public ActivityHelper.EEndCondition EndCondition => endCondition;
        public ActivityHelper.ECodeToRun StartCode => codeToRunAtStart;
        public ActivityHelper.ECodeToRun ContinuousCode => codeToRunContinuously;
        public ActivityHelper.ECodeToRun EndCode => codeToRunAtEnd;

        public delegate void SpecialCode(ITalkerAI ai, SelfActivity activity, AIState aiState);


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            return Random.value * 0.25f * desireabilityFactor; // This will likely need to be tweaked
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


        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, this, aiState);
        }





        
    }


}
