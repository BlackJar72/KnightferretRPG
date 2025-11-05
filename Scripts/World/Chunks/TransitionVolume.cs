using UnityEngine;

namespace kfutils.rpg {


    public class TransitionVolume : AbstractTransition {
        private const float BASE_THICKNESS = 0.05f;
        private const float BASE_THICKNESS_HALF = BASE_THICKNESS * 0.5f;
        private static readonly Vector3 BOX = new Vector3(1.0f, 5.0f, 1.0f); 
        private static readonly Vector3 BASE = new Vector3(1.0f, BASE_THICKNESS, 1.0f);


        void OnTriggerEnter(Collider other) {
            PCMoving pc = other.GetComponent<PCMoving>();
            if(pc != null) MovePlayerCharacter(pc);
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, BOX);
            Gizmos.DrawCube(transform.position + (Vector3.up * BASE_THICKNESS_HALF), BASE);
        }

    }


}
