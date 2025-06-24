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
                case Alignment.player:
                    alignedAction = FriendlyAction;
                    break;
                case Alignment.villages:
                    alignedAction = FriendlyAction;
                    break;
                case Alignment.neutral:
                    alignedAction = NeutralAction;
                    break;
                case Alignment.avoidant:
                    alignedAction = AvoidantAction;
                    break;
                case Alignment.evil:
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
            owner.SetDestination(EntityManagement.playerCharacter.transform.position, 0.5f);
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






    }

}
