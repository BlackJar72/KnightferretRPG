using UnityEngine;

namespace kfutils.rpg {


    public class TransitionInteractable : AbstractTransition, IInteractable
    {
        public void Use(GameObject from) {
            PCMoving pc = from.GetComponent<PCMoving>();
            if(pc != null) MovePlayerCharacter(pc);
        }

    }


}
