using System;
using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/Utility Need Seeker", fileName = "UtilityNeedSeeker", order = 256)]
    public class NeedSeekState : AIState
    {
        private delegate void CurrentAction();
        private CurrentAction currentAction;

        private IActivityObject activityObject;
        private float activityTimer;
        private ActivityChooser chooser;

        private ITalkerAI entity;


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

        }


        public override void Act()
        {
            currentAction();
        }


        public void ChooseActivity()
        {
            ActivityHolder activity = entity.ChooseNeedActivity();
            SetCurrentActivity(activity);
        }


        private void SetCurrentActivity(ActivityHolder activityHolder)
        {
            if (activityHolder != null)
            {
                activityTimer = 0.0f;
                activityObject = activityHolder.ActivityObject;
                currentAction = StartSeekLocation;
            }
        }


        /*
        The methods below works for simple things, objects in the world that directly provide for 
        exactly one need.  However, a lot of improvement is needed to allow this wot work with 
        items that provide more than one need (e.g., the keg used for testing might provide both 
        hunger and enjoyment).  
        
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

        I'd prefer to do all this without adding a bunch of conditionals that would be run every frame while the 
        object or item is in use (notably in the due activity method); how or if I can accomplish this without 
        loading up conditions I'm not sure.  I fear I meay have to use them, thus decreasing the efficieny of 
        item use and making this AI type significantly more costly to run due to reseting the instruction 
        queue when the condition is guessed incorrectly by the CPU.  
        */


        public void StartActivity()
        {
            entity.PlayAction(activityObject.UseAction.mask, activityObject.UseAction.anim);
            activityTimer = Time.time + activityObject.TimeToDo;
            if (activityObject.ActivityType == EObjectActivity.NEED_CONTINUOUS)
            {
                currentAction = DoActivity;
            }
            else
            {
                entity.GetNeed(activityObject.GetNeed).Add(activityObject.Satisfaction);
                currentAction = WaitUntilDone;
            }
        }


        public void WaitUntilDone()
        {
            if (Time.time > activityTimer)
            {
                currentAction = ChooseActivity;
            }
        }


        public void DoActivity()
        {
            entity.GetNeed(activityObject.GetNeed)
                    .Add((activityObject.Satisfaction / activityObject.TimeToDo) * Time.deltaTime);
            if (Time.time > activityTimer)
            {
                currentAction = ChooseActivity;
            }
        }


        private void StartSeekLocation()
        {
            if (activityObject is ActivityProp prop)
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
            if ((activityObject is ActivityProp prop) && entity.AtLocation(prop.ActorLocation))
            {
                currentAction = StartActivity;
            }
        }


    


    }


}
