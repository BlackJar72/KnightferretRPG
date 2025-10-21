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
        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState) => parent.ShouldEndActivity(ai, aiState);
        public void RunStartCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunStartCode(ai, aiState);
        public void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunContinuousCode(ai, aiState);
        public void RunEndCode(ITalkerAI ai, NeedSeekState aiState) => parent.RunEndCode(ai, aiState);
        #endregion parent property reference 

        public bool Available { get => available; set => available = value; }
        public Transform ActorLocation => useLoction;


    }
    #endregion ActivitySlot 



    public class ActivityGroup : MonoBehaviour, IInWorld, IHaveStringID
    {

        [SerializeField] string id;
        [SerializeField] ActivitySlot[] slots;

        public string ID => id;
        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);

        public AbstractAction UseAction => throw new System.NotImplementedException(); 
        public float Satisfaction => throw new System.NotImplementedException(); 
        public float DesireabilityFactor => throw new System.NotImplementedException(); 
        public float TimeToDo  => throw new System.NotImplementedException(); 
        public ENeeds GetNeed => throw new System.NotImplementedException(); 
        public EObjectActivity ActivityType => throw new System.NotImplementedException(); 
        public ActivityHelper.ECodeToRun StartCode => throw new System.NotImplementedException(); 
        public ActivityHelper.ECodeToRun ContinuousCode => throw new System.NotImplementedException(); 
        public ActivityHelper.ECodeToRun EndCode => throw new System.NotImplementedException(); 
        public ActivityHelper.EEndCondition EndCondition => throw new System.NotImplementedException();


        internal ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }


        internal float GetUtility(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }


        internal void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }


        internal void RunEndCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }


        internal void RunStartCode(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }


        internal bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            throw new System.NotImplementedException();
        }



    }



}
