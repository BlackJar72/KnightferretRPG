using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/IdleLook", fileName = "Idle Look", order = 10)]
    public class IdleLook : AIState
    {

        private delegate void AlignedAction();
        private AlignedAction alignedAction;


        public override void Init(EntityActing character)
        {
            base.Init(character);
            switch (character.AL)
            {
                case Disposition.friendly:
                    alignedAction = FriendlyAction;
                    break;
                case Disposition.neutral:
                    alignedAction = NeutralAction;
                    break;
                case Disposition.avoidant:
                    alignedAction = AvoidantAction;
                    break;
                case Disposition.hostile:
                    alignedAction = HostileAction;
                    break;
                default:
                    alignedAction = NeutralAction;
                    break;
            }
        }


        public override void Act()
        {
            alignedAction();
        }


        private void HostileAction()
        {
            //Debug.Log("HostileAction() by " + owner.GetName());
            if (owner.CanSeeEntity(EntityManagement.playerCharacter)) owner.BasicStates.SetState(AIStateID.aggro);

        }


        private void FriendlyAction()
        {
            //Debug.Log("FriendlyAction() by " + owner.GetName());
        }


        private void NeutralAction()
        {
            // Do nothing
        }


        private void AvoidantAction()
        {

        }


        public override void StateEnter() {/*Do Nothing*/}
        public override void StateExit() {/*Do Nothing*/}
        
    }

}
