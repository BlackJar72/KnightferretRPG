using UnityEngine;


namespace kfutils.rpg
{

    /*
    There are relatively few world objects ("props") that would make sense in the setting of 
    the actual planned game that would just supply a need directly -- less so than in a 
    modern setting where appliances like TVs exist.  It seems like most need suppliers should 
    actually be item suppliers that advertise the values of the items they supply, thus allowing 
    those items to be used with the next behavior choice.  This would work better with action 
    planning implemented where, for example, hunger might trigger eat -> get food.  However, 
    placing the item in the inventory ought to be a start to be select on the next AI call 
    should work.  However, a system for thise still needs to be set up.
    */


    /// <summary>
    /// Reperesnts items in the world that you may want NPC to interact with. The 
    /// word "prop" in the name is used to avoid confusion and because "object" is 
    /// used elsewhere. 
    /// </summary>
    public class ActivityProp : MonoBehaviour, IActivityObject, IHaveStringID, IInWorld, IActivityProp
    {
        [SerializeField] string id;
        [SerializeField] ENeeds theNeeds;
        [Tooltip("Should be NEED_DISCRETE or NEED_CONTINUOUS")][SerializeField] protected EObjectActivity activityType;
        [SerializeField] EActivityRun activityRun;
        [SerializeField] ECodeToRun codeToRun;
        [SerializeField] AbstractAction useAction;
        [Range(0.0f, 1.0f)][SerializeField] float satisfaction;
        [Range(0.0f, 2.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] Transform actorLocation;
        [SerializeField] bool shareable = false;
        [SerializeField] ActivityHelper.EEndCondition endCondition;

        public bool available = true;
        public bool Available { get => available; set => available = value;  }

        protected float desireability;

        public ENeeds TheNeed => theNeeds;
        public float Satisfaction => satisfaction;
        public AbstractAction UseAction => useAction;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public Transform ActorLocation => actorLocation;
        public bool Shareable => shareable;
        public string ID => id;
        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);
        public ENeeds GetNeed => theNeeds;
        public EObjectActivity ActivityType => activityType;
        public EActivityRun ActivityCode => activityRun;
        public ActivityHelper.EEndCondition EndCondition => endCondition;
        

        public delegate void SpecialCode(ITalkerAI ai, ActivityProp activity, AIState aiState);


        protected void OnEnable()
        {
            WorldManagement.OnPostLoad += OnPostLoad;
        }


        protected void OnDisable()
        {
            WorldManagement.OnPostLoad -= OnPostLoad;
        }


        private void OnPostLoad()
        {
            GetChunkManager.AddActivityProp(this);
        }


        public virtual float GetUtility(ITalkerAI entity)
        {
            if (available || shareable)
            {
                desireability = 0.0f;
                if ((theNeeds & ENeeds.ENERGY) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.ENERGY));
                if ((theNeeds & ENeeds.FOOD) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.FOOD));
                if ((theNeeds & ENeeds.SOCIAL) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.SOCIAL));
                if ((theNeeds & ENeeds.ENJOYMENT) > 0) desireability = Mathf.Max(desireability, GetUtilityForNeed(entity, ENeedID.ENJOYMENT));
                desireability /= Mathf.Sqrt((entity.GetTransform.position - actorLocation.position).magnitude) + 1;
                return desireability;
            }
            else
            {
                return 0.0f;
            }
        }


        private float GetUtilityForNeed(ITalkerAI entity, ENeedID theNeed)
        {
            float desireability = (satisfaction * desireabilityFactor)
                                + Mathf.Sqrt((satisfaction * desireabilityFactor) / (timeToDo + 5f) + 1) - 1;
            desireability *= entity.GetNeed(theNeed).GetDrive();
            return desireability;
        }


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public void RunSpecialCode(ITalkerAI ai, AIState aiState)
        {
            effects[(int)codeToRun](ai, this, aiState);
        }


        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, this, aiState);
        }


        #region Special Code
        /*******************************************************************************************/
        /*                                 SPECIAL CODE                                            */
        /*******************************************************************************************/


        public enum ECodeToRun
        {
            NONE = 0,
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        private static SpecialCode[] effects = new SpecialCode[]{
            DoNothing,
        };


        private static void DoNothing(ITalkerAI ai, ActivityProp activity, AIState aiState) { }





        #endregion


    }



}
