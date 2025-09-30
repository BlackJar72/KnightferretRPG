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
            activityTimer = 0.0f;
            activityObject = activityHolder.ActivityObject;
            currentAction = StartSeekLocation;
        }


        public void StartActivity()
        {
            entity.PlayAction(activityObject.UseAction.mask, activityObject.UseAction.anim);
            currentAction = DoActivity;            
        }


        public void DoActivity()
        {
            
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
            if ((activityObject is ActivityProp prop)
                    && entity.AtLocation(prop.ActorLocation)) currentAction = StartActivity;
        }


    


    }


}
