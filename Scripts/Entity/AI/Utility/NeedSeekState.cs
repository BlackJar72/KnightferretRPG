using System;
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
        private RingDeque<ActivityHolder> activityQueue;

        private ITalkerAI entity;


        public override void Init(EntityActing character)
        {
            base.Init(character);
            if (character is ITalkerAI ai)
            {
                chooser = ai.NeedChooser;
                entity = ai;
                activityQueue = new(MAX_QUEUE);
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
            EndActivity();
        }


        public override void Act()
        {
            currentAction();
        }



        public void ChooseActivity()
        {
            if (!activityQueue.IsEmpty) SetCurrentActivity(activityQueue.PopFront());
            else
            {
                ActivityHolder activity = entity.ChooseNeedActivity();
                SetCurrentActivity(activity);
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
            if (activity.ActivityObject.ActivityCode == EActivityRun.START) activity.ActivityObject.RunSpecialCode(entity, this);
            if (activity.ActivityObject is ActivityProp prop) prop.available = false;
            if (activity.ActivityObject is ActivityItem item)
            {
                entity.EquipItem(activity.itemStack);
            }
            entity.PlayAction(activity.ActivityObject.UseAction.mask, activity.ActivityObject.UseAction.anim);
        }


        public void WaitUntilDone()
        {
            if (Time.time > activityTimer)
            {
                EndActivity();
            }
            if (activity.ActivityObject.ActivityCode == EActivityRun.CONTINUOUS) activity.ActivityObject.RunSpecialCode(entity, this);
        }


        public void DoActivity()
        {
            entity.GetNeeds.AddToNeeds(activity.ActivityObject.GetNeed,
                                    (activity.ActivityObject.Satisfaction / activity.ActivityObject.TimeToDo) * Time.deltaTime);
            if (Time.time > activityTimer)
            {
                EndActivity();
            }
            if (activity.ActivityObject.ActivityCode == EActivityRun.CONTINUOUS) activity.ActivityObject.RunSpecialCode(entity, this);
        }


        private void EndActivity()
        {
            if (activity.ActivityObject is ActivityProp prop)
            {
                prop.available = true;
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
            if (activity.ActivityObject is ActivityProp prop)
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
            if ((activity.ActivityObject is ActivityProp prop) && entity.AtLocation(prop.ActorLocation))
            {
                currentAction = StartActivity;
            }
        }


    


    }


}
