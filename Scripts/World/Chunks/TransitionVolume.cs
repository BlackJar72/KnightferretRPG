using UnityEngine;

namespace kfutils.rpg {


    public class TransitionVolume : AbstractTransition {


        void OnTriggerEnter(Collider other) {
            PCMoving pc = other.GetComponent<PCMoving>();
            if(pc != null) MovePlayerCharacter(pc);
        }

    }


}
