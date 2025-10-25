using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    #region ActivitySlot
    [System.Serializable]
    public class ActivitySlot : IActivityObject, IActivityProp
    {
        public enum State
        {
            EMPTY, 
            WAITING,
            USING
        }

        private GroupActivity parent;
        public Transform useLoction;
        public ITalkerAI user;
        public State state = State.EMPTY;

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

        public bool Available { get => state == State.EMPTY; set => state = State.WAITING; }
        public Transform ActorLocation => useLoction;
        public GroupActivity Parent => parent;
        public bool InUse => (user != null) && (state == State.USING);
        public bool NotInUse => (user == null) || (state != State.USING);


        internal void SetParent(GroupActivity parent)
        {
            if (this.parent == null) this.parent = parent; 
        }


        public void Free()
        {
            state = State.EMPTY;
            user = null;
        }


        public void Assign(ITalkerAI ai)
        {
            user = ai;
            state = State.WAITING;
        }
        

        public void StartActivity(ITalkerAI participant)
        {
            state = State.USING;
            user = participant;
        }


    }
    #endregion ActivitySlot 



    public class GroupActivity : MonoBehaviour, IInWorld, IHaveStringID, ISerializationCallbackReceiver, IActivityObject, IActivityProp
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
        [SerializeField] int maxParticipants;
        [SerializeField] int minParticipants;
        [SerializeField] float waitTimeOut;
        [SerializeField] private bool waitingToStart = true;
        [SerializeField] private bool completed = false;
        [SerializeField] private bool hasFirstTaker;

        private Coroutine waitForTimeout;

        public string ID => id;
        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);

        public AbstractAction UseAction => useAction;
        public float Satisfaction { get { if (waitingToStart) return 0.0f; else return satisfaction; } }
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public ENeeds GetNeed => needs; 
        public EObjectActivity ActivityType => activityType;
        public ActivityHelper.ECodeToRun StartCode => startCode; 
        public ActivityHelper.ECodeToRun ContinuousCode => continuousCode; 
        public ActivityHelper.ECodeToRun EndCode => endCode; 
        public ActivityHelper.EEndCondition EndCondition => endCondition;
        public float WaitTimeOut => waitTimeOut;
        public bool Completed => completed;

#if UNITY_EDITOR
        public int Participants {
            get
            {
                int result = 0;
                for (int i = 0; i < slots.Length; i++)
                    if ((slots[i].state == ActivitySlot.State.USING) && (slots[i].user != null)) result++;
                participants = result;
                return result;  
            }   
        }
        public int participants;
#else
        public int Participants {
            get
            {
                int result = 0;
                for (int i = 0; i < slots.Length; i++)
                    if ((slots[i].state == ActivitySlot.State.USING) && (slots[i].user != null)) result++;
                return result;  
            }   
        }
#endif

        protected void Awake()
        {
            for (int i = 0; i < slots.Length; i++) slots[i].SetParent(this);
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


        public bool Available
        {
            get
            {
                if (Participants >= maxParticipants) return false;
                for (int i = 0; i < slots.Length; i++) if (slots[i].Available) return true;
                return false;
            }
            set {/*Do Nothing; this is handled by the slots*/}
        }


        public Transform ActorLocation => throw new System.NotImplementedException();


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
#if UNITY_EDITOR
            Debug.Log("I'm a group activity!  Choosen by " + entity.ID + " at " + Time.time + " (Frame " + WorldTime.Frame + ") "); 
#endif
            return new ActivityHolder(this, GetUtility(entity));
        }


        public void OnAfterDeserialize() { }
        public void OnBeforeSerialize()
        {
            maxParticipants = Mathf.Max(Mathf.Min(maxParticipants, slots.Length), 0);
            minParticipants = Mathf.Max(Mathf.Min(minParticipants, maxParticipants), 0);
        }


        public float GetUtility(ITalkerAI entity)
        {
            return GetSatisfactionForNeed(entity) * desireabilityFactor 
                   * Mathf.Sqrt((entity.GetTransform.position - transform.position).magnitude);
        }


        public void RunStartCode(ITalkerAI ai, NeedSeekState aiState) => ActivityHelper.RunStartCode(ai, this, aiState);
        public void RunContinuousCode(ITalkerAI ai, NeedSeekState aiState) => ActivityHelper.RunContinuousCode(ai, this, aiState);
        public void RunEndCode(ITalkerAI ai, NeedSeekState aiState) => ActivityHelper.RunEndCode(ai, this, aiState);
        public bool ShouldEndActivity(ITalkerAI ai, NeedSeekState aiState) => ActivityHelper.ShouldEndActivity(ai, this, aiState);


        public bool ShouldExceptInvite(ITalkerAI entity, GroupActivity existing = null)
        {
            if (existing == null) return Random.value < GetUtility(entity);
            else
            {
                float roll = Random.value;
                float utility = GetUtility(entity);
                float otherUtility = existing.GetUtility(entity);
                return (roll < utility) && ((roll * (utility + otherUtility)) < utility);
            }
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


        public ActivityHolder GetSlotForParticipant(ITalkerAI ai, NeedSeekState aiState)
        {
            int maxUsers = Mathf.Min(maxParticipants, slots.Length);
            List<ActivitySlot> availableSlots = new(maxUsers);
            for (int i = 0; i < maxUsers; i++)
            {
                if (slots[i].Available) availableSlots.Add(slots[i]);
            }
            if (availableSlots.Count > 0)
            {
                if(ai is EntityLiving entity)
                {
                    ChunkManager chunk = entity.GetChunkManager;
                    foreach(ITalkerAI other in chunk.ActivityNPCs)
                    {
                        if((other != ai) && (other.CurrentAIState is NeedSeekState needSeek))
                        {
                            needSeek.BeInvitedToActivity(this); 
                        }
                    }
                }
                availableSlots.Shuffle();
                ActivitySlot selected = availableSlots[0];
                selected.Assign(ai);
                Debug.Log("GetSlotForParticipant found slot for " + ai.ID + " on frame " + WorldTime.Frame + ".");
                return new ActivityHolder(selected, 1.0f);
            }
            return null;
        }


        public void AbandonActivity(ActivitySlot activitySlot, ITalkerAI ai)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == activitySlot)
                {
                    slots[i].Free();
                    return;
                }
            }
        }


        public void StartParticipation(ActivitySlot activitySlot, ITalkerAI user)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == activitySlot)
                {
                    activitySlot.StartActivity(user);
                    if (waitingToStart && (Participants == 1)) waitForTimeout = StartCoroutine(WaitForTimeout());
                    waitingToStart = waitingToStart && (Participants < minParticipants);
                    if (!waitingToStart && (waitForTimeout != null)) StopWaitForTimeone();
                    return;
                }
            }
        }


        private IEnumerator WaitForTimeout()
        {
            yield return new WaitForSeconds(waitTimeOut);
            foreach(ActivitySlot slot in slots)
            {
                if((slot.user != null) && (slot.user.CurrentAIState is NeedSeekState seeker))
                {
                    seeker.QuitSocialActivity();   
                }
                slot.Free();                 
            }
            waitingToStart = true;
            completed = false;
        }
        

        private void StopWaitForTimeone()
        {
            StopCoroutine(waitForTimeout);
            waitForTimeout = null;
        }


        public void StopParticipation(ActivitySlot activitySlot)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == activitySlot)
                {
                    slots[i].Free();
                    completed = (Participants < minParticipants) & !waitingToStart;
                    if (completed && (Participants == 0))
                    {
                        if(waitForTimeout != null) StopWaitForTimeone(); 
                        waitingToStart = true; 
                        completed = false;
                    } 
                    return;
                }
            }
        }
        



    }



}
