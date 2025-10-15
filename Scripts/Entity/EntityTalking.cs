using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    public class EntityTalking : EntityActing, ITalkerAI, IActivityObject, IInWorld, IActivityProp
    {

        [SerializeField] Personality personality;
        [SerializeField] CoreNeeds needs;
        [SerializeField] ActivityChooser needChooser;
        [SerializeField] SelfActivity[] selfActivities;

        public ActivityChooser NeedChooser => needChooser;
        public CoreNeeds GetNeeds => needs;
        public SelfActivity[] SelfActivities => selfActivities;
        public Personality AIPersonality => personality;


        #region Social Activity Object
        /*********************************************************************************************/
        /*                              SOCIAL ACTIVITY OBJECT                                       */
        /*********************************************************************************************/

        // FIXME: Should this even be here?  There may be a better way to do this, if I can make it work
        //        at all.  No, start with shared activities, then have them create a temporary group 
        //        activity for conversation (possibly a special sub-class).

        [SerializeField] AbstractAction socialActions;
        [SerializeField] Transform actorLocation;

        public bool available = true, shareable = true;

        public AbstractAction UseAction => socialActions;
        public float Satisfaction => (((attributes.baseStats.Charisma + personality.Extroverted) * 0.25f) + personality.Sensitive) * 0.01f;
        public float DesireabilityFactor => ((attributes.baseStats.Charisma + personality.Extroverted) * 0.01f) + 0.8f;
        public float TimeToDo => 2.0f;
        ENeeds IActivityObject.GetNeed => ENeeds.SOCIAL;
        public EObjectActivity ActivityType => EObjectActivity.NEED_CONTINUOUS;
        public EActivityRun ActivityCode => EActivityRun.END; // FIXME? See comment below on needed special codes. 
        public Transform ActorLocation => actorLocation;
        public bool Available { get => available; set => available = value; }

        public ActivityHolder GetActivityOption(ITalkerAI entity) => new ActivityHolder(this, GetUtility(entity));


        public float GetUtility(ITalkerAI entity)
        {
            float desirability = (Satisfaction + (entity.AIPersonality.Extroverted * 0.01f)) * personality.Compatibility(entity.AIPersonality);
            desirability *= 2.0f; // This will be replace by desirability += relationship effect; this stand-in compesates for the lack of this addition.
            // TODO: Factor in relationship status (perhaps make it the main source) once relationship system is done.
            desirability *= entity.GetNeed(ENeedID.SOCIAL).GetDrive();
            desirability /= Mathf.Sqrt((entity.GetTransform.position - actorLocation.position).magnitude) + 1;
            return desirability;
        }

        // FIXME: I need special code for both the beginning (to insert a be interacted with action into queue) 
        //        and the end (to determine if more interaction should occur, pre-opting other actions); perhap, 
        //        though, the decision to continue to be up to the target. 

        public void RunSpecialCode(ITalkerAI ai, AIState aiState) {}


        public bool ShouldEndActivity()
        {
            return true; // FIXME/TODO: Actually figure out a legitimate result.
        }

        // NOTE ON RELATIONSHIP SYSTEM: Do I really need the kind of complex, dynamic relationship system between NPCs I was 
        // planning on for the life simulator project.  For a long running, "live in the world" game with strong life simulation 
        // elements this is might be yes, allowing the payer to see their community evolve around them.  For a simpler, shorter 
        // running RPG, probably not, and such things could just be predefined numbers that do not change.  Of course, since 
        // this is officially a framework for making such games I could include both with a common interface (maybe and/or abstract 
        // parent) which an easily be swapped in some way, possibly even be commenting a line of code and uncommenting its 
        // neighbor. 

        /*********************************************************************************************/
        /*-------------------------------------------------------------------------------------------*/
        /*********************************************************************************************/
        #endregion Social Activity Object 


        protected override void Awake()
        {
            base.Awake();
            needs.Init(this);
        }


        protected override void StoreData()
        {
            base.StoreData();
            // TODO: Store Data
        }


        protected override void LoadData()
        {
            base.LoadData();
            // TODO: Store Data
        }


        protected override void Update()
        {
            base.Update();
            needs.UpdateNeeds();
        }


        public Need GetNeed(ENeedID need)
        {
            return needs.GetNeed(need);
        }


        public ActivityHolder ChooseNeedActivity()
        {
            ChunkManager chunk = GetChunkManager;
            List<IActivityObject> props = chunk.ActivityProps;
            List<ActivityHolder> itemsDiscrete = inventory.GetActivities(EObjectActivity.NEED_DISCRETE, this);
            List<ActivityHolder> itemsConinuous = inventory.GetActivities(EObjectActivity.NEED_CONTINUOUS, this);
            needChooser.PopulateActivityList(this, props);
            needChooser.AddToList(itemsDiscrete, itemsConinuous);
            return needChooser.Choose();
        }





    }


}
