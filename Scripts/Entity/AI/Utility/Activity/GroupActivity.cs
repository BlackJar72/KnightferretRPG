using UnityEngine;


namespace kfutils.rpg
{

    #region ActivitySlot
    [System.Serializable]
    public struct ActivitySlot : IActivityObject, IActivityProp
    {
        private ActivityGroup parent;
        public Transform useLoction;
        public ITalkerAI user;
        public bool available;

        #region parent property reference
        public AbstractAction UseAction => parent.UseAction; 
        public float Satisfaction => parent.Satisfaction;
        public float DesireabilityFactor => parent.DesireabilityFactor;
        public float TimeToDo => parent.TimeToDo;
        public ENeeds GetNeed => parent.GetNeed;
        public EObjectActivity ActivityType => parent.ActivityType;
        public ActivityHelper.ECodeToRun StartCode => parent.StartCode;
        public ActivityHelper.ECodeToRun ContinuousCode => parent.ContinuousCode;
        public ActivityHelper.ECodeToRun EndCode => parent.EndCode;
        public ActivityHelper.EEndCondition EndCondition => parent.EndCondition;

        public float GetUtility(ITalkerAI entity) => parent.GetUtility(entity);
        public ActivityHolder GetActivityOption(ITalkerAI entity) => parent.GetActivityOption(entity);
        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState) => parent.ShouldEndActivity(ai, this, aiState);
        public void RunStartCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunStartCode(ai, this, aiState);
        public void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunContinuousCode(ai, this, aiState);
        public void RunEndCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunEndCode(ai, this, aiState);
        #endregion parent property reference 

        public bool Available { get => available; set => available = value; }
        public Transform ActorLocation => useLoction;


    }
    #endregion ActivitySlot 



    public class ActivityGroup : MonoBehaviour, IInWorld, IHaveStringID
    {

        [SerializeField] string id;
        [SerializeField] ActivitySlot[] slots;
        [SerializeField] ENeeds needs;
        [SerializeField] float timeToDo;
        [SerializeField] float satisfaction;
        [SerializeField] float desireabilityFactor;
        [SerializeField] AbstractAction useAction;
        [SerializeField] EObjectActivity activityType;
        [SerializeField] ActivityHelper.ECodeToRun startCode;
        [SerializeField] ActivityHelper.ECodeToRun continuousCode;
        [SerializeField] ActivityHelper.ECodeToRun endCode;
        [SerializeField] ActivityHelper.EEndCondition endCondition;

        public string ID => id;
        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);

        public AbstractAction UseAction => useAction; 
        public float Satisfaction => satisfaction; 
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public ENeeds GetNeed => needs; 
        public EObjectActivity ActivityType => activityType;
        public ActivityHelper.ECodeToRun StartCode => startCode; 
        public ActivityHelper.ECodeToRun ContinuousCode => continuousCode; 
        public ActivityHelper.ECodeToRun EndCode => endCode; 
        public ActivityHelper.EEndCondition EndCondition => endCondition;


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }


        public float GetUtility(ITalkerAI entity)
        {
            return GetSatisfactionForNeed(entity) * desireabilityFactor 
                   * Mathf.Sqrt((entity.GetTransform.position - transform.position).magnitude);
        }


        public void RunStartCode(ITalkerAI ai, ActivitySlot slot, NeedSeekState aiState)
        {
            ActivityHelper.RunStartCode(ai, slot, aiState);
        }


        public void RunContinuousCode(ITalkerAI ai, ActivitySlot slot, NeedSeekState aiState)
        {
            ActivityHelper.RunContinuousCode(ai, slot, aiState);
        }


        public void RunEndCode(ITalkerAI ai, ActivitySlot slot, NeedSeekState aiState)
        {
            ActivityHelper.RunEndCode(ai, slot, aiState);
        }


        public bool ShouldEndActivity(ITalkerAI ai, ActivitySlot slot, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, slot, aiState);
        }


        public bool ShouldExceptInvite(ITalkerAI entity)
        {
            return Random.value < GetUtility(entity);
        }
        

        private float GetSatisfactionForNeed(ITalkerAI entity)
        {
            float result = 0.0f;
            if ((needs & ENeeds.ENERGY) > 0) result = Mathf.Max(result, satisfaction * entity.GetNeed(ENeedID.ENERGY).GetDrive());
            if ((needs & ENeeds.FOOD) > 0) result = Mathf.Max(result, satisfaction * entity.GetNeed(ENeedID.FOOD).GetDrive());
            if ((needs & ENeeds.SOCIAL) > 0) result = Mathf.Max(result, satisfaction * entity.GetNeed(ENeedID.SOCIAL).GetDrive());
            if ((needs & ENeeds.ENJOYMENT) > 0) result = Mathf.Max(result, satisfaction * entity.GetNeed(ENeedID.ENJOYMENT).GetDrive());
            return result;
        }






    }



}
