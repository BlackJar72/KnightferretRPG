using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg
{

    public class ActivityItemProvider : MonoBehaviour, IActivityObject, IHaveStringID, IInWorld, IActivityProp
    {
        [SerializeField] string id;
        [SerializeField] ItemStack.ProtoStack[] items;
        [SerializeField] SelfActivity preUseActivity;
        [Range(0.0f, 2.0f)][SerializeField] protected float desireabilityFactor = 1.0f;
        [SerializeField] protected EActivityRun activityRun;
        [SerializeField] protected ECodeToRun codeToRun;
        [SerializeField] protected AbstractAction useAction;
        [SerializeField] protected float timeToDo;
        [SerializeField] protected Transform actorLocation;
        [SerializeField] protected bool shareable = false;
        [SerializeField] ActivityHelper.EEndCondition endCondition;

        public bool available = true;
        public bool Available { get => available; set => available = value;  }

        protected float desireability;

        public ItemStack.ProtoStack[] Items => items;
        public SelfActivity PreUseActivity => preUseActivity;
        public ActivityHolder PreUseActivityHolder => new(preUseActivity, 1.0f);
        public AbstractAction UseAction => useAction;
        public float TimeToDo => timeToDo;
        public EObjectActivity ActivityType => EObjectActivity.NEED_DISCRETE;
        public EActivityRun ActivityCode => activityRun;
        public string ID => id;
        public Transform ActorLocation => actorLocation;
        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);
        public float Satisfaction => FindHighestSatisfaction();
        public float DesireabilityFactor => FindHighestDesirability();
        public ENeeds GetNeed => FindNeeds();
        public ActivityHelper.EEndCondition EndCondition => endCondition;


        private ENeeds FindNeeds()
        {
            ENeeds result = ENeeds.NONE;
            foreach (ItemStack.ProtoStack protoStack in items) result |= protoStack.item.Prototype.Activity.GetNeed;
            return result;
        }


        private float FindHighestDesirability()
        {
            float result = 0.0f;
            foreach (ItemStack.ProtoStack protoStack in items) result = Mathf.Max(result, protoStack.item.Prototype.Activity.DesireabilityFactor);
            return result;
        }


        private float FindHighestSatisfaction()
        {
            float result = 0.0f;
            foreach (ItemStack.ProtoStack protoStack in items) result = Mathf.Max(result, protoStack.item.Prototype.Activity.Satisfaction);
            return result;
        }


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


        public float GetUtility(ITalkerAI entity)
        {
            desireability = 0.0f;
            foreach (ItemStack.ProtoStack protoStack in Items)
            {
                desireability = Mathf.Max(desireability, protoStack.item.Prototype.Activity.GetUtility(entity));
            }
            desireability /= Mathf.Sqrt((entity.GetTransform.position - actorLocation.position).magnitude) + 1;
            return desireability;
        }


        public ActivityHolder ProvideItem(ITalkerAI ai, AIState aiState)
        {
            List<ActivityHolder> choices = new();
            foreach (ItemStack.ProtoStack protoStack in items)
            {
                choices.Add(new ActivityHolder(protoStack.item.Prototype.Activity,
                                               protoStack.item.Prototype.Activity.GetUtility(ai), 
                                               protoStack.MakeStack()));
            }
            choices.Sort();
            float range = 0.0f;
            for (int i = 0; (i < 6) && (i < choices.Count); i++)
            {
                range += choices[i].Utility;
            }
            range *= range;
            int selection = 0;
            while (range > choices[selection].Utility)
            {
                range -= choices[selection].Utility;
                selection++;
            }
            return choices[selection];
        }


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public bool ShouldEndActivity()
        {
            throw new System.NotImplementedException();
        }


        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState)
        {
            return ActivityHelper.ShouldEndActivity(ai, this, aiState);
        }




        #region Special Code
        /*******************************************************************************************/
        /*                                 SPECIAL CODE                                            */
        /*******************************************************************************************/


        public delegate void SpecialCode(ITalkerAI ai, ActivityItemProvider activity, AIState aiState);


        public void RunSpecialCode(ITalkerAI ai, AIState aiState)
        {
            effects[(int)codeToRun](ai, this, aiState);
        }


        public enum ECodeToRun
        {
            NONE = 0,
            PROVIDE_ITEM = 1,
        }


        /// <summary>
        /// And array of delegate methods for potion effects.  These must be in the same order as the corresponding 
        /// enum constants in order to match them up correctly.  
        /// </summary>
        private static SpecialCode[] effects = new SpecialCode[]{
            DoNothing,
        };


        private static void DoNothing(ITalkerAI ai, ActivityItemProvider activity, AIState aiState) { }





        #endregion


    }


}
