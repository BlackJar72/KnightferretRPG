using System;
using Animancer;
using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/Utility Need Seeker", fileName = "UtilityNeedSeeker", order = 256)]
    public class NeedSeekState : AIState
    {
        private const int MAX_QUEUE = 6;

        private delegate void CurrentAction();
        private CurrentAction currentAction;

        private ActivityHolder activity;
        private float activityTimer;
        private ActivityChooser chooser;
        private RingDeque<ActivityHolder> activityQueue = new(MAX_QUEUE);
        private ITalkerAI entity;
        private AnimancerState animState;

        private GroupActivity potentialGroup;

        private bool notified = false;
        private bool animEnded = false;

        public float ActivityTimer => activityTimer; 

        public bool Notified => notified;
        public bool AnimEnded => animEnded;


        public override void Init(EntityActing character)
        {
            base.Init(character);
            if (character is ITalkerAI ai)
            {
                chooser = ai.NeedChooser;
                entity = ai;
            }
            else
            {
                Debug.LogError("Trying to use NeedSeekState with entity " + character.ID + "(" + character.GetName()
                    + "; Object: " + character.gameObject.name + ") that does not implement ITalkerAI.");
#if UNITY_EDITOR                    
                throw new Exception("Trying to use NeedSeekState with entity " + character.ID + "(" + character.GetName()
                    + "; Object: " + character.gameObject.name + ") that does not implement ITalkerAI.");
#endif
            }
        }


        public override void StateEnter()
        {
            currentAction = ChooseActivity;
        }


        public override void StateExit()
        {
            if(animState != null) animState.Events.OnEnd -= OnAnimEnd;
            if ((activity != null) && (activity.ActivityObject is IActivityProp prop))
            {
                prop.Available = true;
            }
            activityQueue.Clear();
        }


        public override void Pause()
        {
            // Something *MAY* need to be done here, but nothing I can think of right now.
        }


        public override void Resume()
        {
            // May have changed destination to move somewhere else, thus this must be reset 
            currentAction = StartSeekLocation; 
        }


        void OnDisable()
        {
            StateExit();
        }


        public override void Act()
        {
            currentAction();
        }


        public void ChooseActivity()
        {
            if(WorldManagement.WorldLogic == null) return;
            if (!activityQueue.IsEmpty) SetCurrentActivity(activityQueue.PopFront());
            else
            {
                if (potentialGroup != null)
                {
                    ActivityHolder activitySlot = potentialGroup.GetSlotForParticipant(entity, this);
                    if (activitySlot == null)
                    {
                        ActivityHolder activity = entity.ChooseNeedActivity();
                        SetCurrentActivity(activity);
                    }
                    else
                    {
                        if (activity.ActivityObject is ActivitySlot slot) slot.Parent.AbandonActivity(slot, entity);
                        SetCurrentActivity(activitySlot);
                    }
                    potentialGroup = null; 
                }
                else
                {
                    ActivityHolder activity = entity.ChooseNeedActivity();
                    SetCurrentActivity(activity);
                }
            }
        }


        private void SetCurrentActivity(ActivityHolder activityHolder)
        {
            if (activityHolder != null)
            {
                activityTimer = 0.0f;
                activity = activityHolder;
                currentAction = StartSeekLocation;
            }
        }


        public void QueueActivityBack(ActivityHolder activity) => activityQueue.AddBackSafe(activity);
        public void QueueActivityFront(ActivityHolder activity) => activityQueue.AddFront(activity);


        public void ReplaceNextQueued(ActivityHolder activity)
        {
            if (activityQueue.IsEmpty) activityQueue.AddFront(activity);
            else activityQueue.ReplaceFront(activity);
        }


        public bool BeInvitedToActivity(GroupActivity activity)
        {
#if UNITY_EDITOR
            //Debug.Log(entity.ID + " was invite on frame " + WorldTime.Frame + ". ");
#endif
            bool accepted = activity.ShouldExceptInvite(entity, potentialGroup);
            if (accepted) potentialGroup = activity;
            return accepted;
        }


        public void QuitSocialActivity()
        {
            potentialGroup = null;
            if((activity.ActivityObject is ActivitySlot) || (activity.ActivityObject is GroupActivity))
            {
                EndActivity();
            }
            currentAction = ChooseActivity;
        }


        /*         
        In addition, more needs to be considered for item providers, objects in the world that provide 
        items filling a need rather than filling the need themself.  First, they need to give the item 
        and then have the character use it; this could be done through the activity queue I have planned, 
        or better yet the "side chains" idea I had, that would hold a series of activities to be loaded 
        to the front of the queue. Then, the chains may be excessive, as activities should not otherwise
        be queued ahead of time for the purpose of this game.  In addition to using the item, in most 
        cases another action would need to be performed first, such as wandering off or looking for a seat, 
        first; the character should not stand in front of the keg (blocking it for others) until done drinking!

        Another issue with item providers as relates to the possibility that the same provider might provide 
        more than one item (e.g., merchants would be a special type of item provider while in a working state). 
        This would mean that all provided items would need to be considered, but then getting the item from 
        the provider would need to be queued.
        */

        // TODO: Need to makes properly use items (now that it should be able to selection them).


        public void StartActivity()
        {
            activityTimer = Time.time + activity.ActivityObject.TimeToDo;
            if (activity.ActivityObject.ActivityType == EObjectActivity.NEED_CONTINUOUS)
            {
                currentAction = DoActivity;
            }
            else
            {
                entity.GetNeeds.AddToNeeds(activity.ActivityObject.GetNeed, activity.ActivityObject.Satisfaction);
                currentAction = WaitUntilDone;
            }
            activity.ActivityObject.RunStartCode(entity, this);
            if (activity.ActivityObject is IActivityProp prop) prop.Available = false;
            if (activity.ActivityObject is ActivityItem item)
            {
                entity.EquipItem(activity.itemStack);
            }
            if (activity.ActivityObject is ActivitySlot activitySlot)
            {
                activitySlot.Parent.StartParticipation(activitySlot, entity);
            }
            if (activity.ActivityObject is ActivityItemProvider provider)
            {
                ActivityHolder providedItem = provider.ProvideItem(entity, this);
                entity.CharInventory.AddNewEquiptItem(providedItem.itemStack);
                QueueActivityFront(providedItem);
                QueueActivityFront(provider.PreUseActivityHolder);
            }
            if (activity.ActivityObject.UseAction == null)
            {
                entity.StopAction();
            }
            else
            {
                animState = entity.PlayAction(activity.ActivityObject.UseAction.mask, activity.ActivityObject.UseAction.anim);
                animState.Events.OnEnd += OnAnimEnd;
            }
        }
        

        public void OnAnimEnd()
        {
            animState.Events.OnEnd -= OnAnimEnd;
            if(activity.ActivityObject.EndCondition == ActivityHelper.EEndCondition.ANIM_END)
            {
                currentAction = EndActivity;
            }
            animEnded = true;
        }


        public void WaitUntilDone()
        {
            if ((activity.ActivityObject.ShouldEndActivity(entity, this)) 
                        /*|| ((activity.ActivityObject is ActivitySlot activitySlot) && activitySlot.NotInUse)*/)
            {
                EndActivity();
            }
            activity.ActivityObject.RunContinuousCode(entity, this);
        }


        public void DoActivity()
        {
            entity.GetNeeds.AddToNeeds(activity.ActivityObject.GetNeed,
                                    (activity.ActivityObject.Satisfaction / activity.ActivityObject.TimeToDo) * Time.deltaTime);
            if ((activity.ActivityObject.ShouldEndActivity(entity, this)) 
                        /*|| ((activity.ActivityObject is ActivitySlot activitySlot) && activitySlot.NotInUse)*/)
            {
                EndActivity();
            }
            activity.ActivityObject.RunContinuousCode(entity, this);
        }


        private void EndActivity()
        {
            activity.ActivityObject.RunEndCode(entity, this);
            if (activity.ActivityObject is IActivityProp prop)
            {
                prop.Available = true;
            }
            if (activity.ActivityObject is ActivitySlot activitySlot)
            {
                activitySlot.Parent.StopParticipation(activitySlot);
            }
            if (activity.ActivityObject is ActivityItem item)
            {
                // FIXME / TODO: Have item be used (but when exactly)
                if (activity.itemStack.item.EquiptItem is IUsable)
                {
                    entity.UseItem(activity.itemStack.item.EquiptType);
                }
                if (activity.itemStack.stackSize > 0)
                {
                    entity.UnequipItem(activity.itemStack);
                }
            }
            currentAction = ChooseActivity;
            entity.StopAction();
        }


        private void StartSeekLocation()
        {
            animEnded = notified = false;
            if(activity.ActivityObject is GroupActivity group)
            {
                ActivityHolder activitySlot = group.GetSlotForParticipant(entity, this);
                if (activitySlot != null) SetCurrentActivity(activitySlot);
                else
                {
                    potentialGroup = null; 
                    currentAction = ChooseActivity;
                }
                return;  // Should start seek for the slot on next frame
            }
            if (activity.ActivityObject is IHaveUseLocation prop)
            {
                owner.SetDestination(prop.ActorLocation.position);
                currentAction = SeekActivityLocation;
            }
            else
            {
                currentAction = StartActivity;
            }
        }


        public void SeekActivityLocation()
        {
            if ((activity.ActivityObject is IHaveUseLocation prop) && entity.AtLocation(prop.ActorLocation))
            {
                currentAction = StartActivity;
            }
        }


        public void BeNotified()
        {
            notified = true;
            if (activity.ActivityObject.EndCondition == ActivityHelper.EEndCondition.NOTIFIED)
            {
                currentAction = EndActivity;
            }
        }
        

    }


}
