using UnityEngine;
using UnityEngine.SceneManagement;


namespace kfutils.rpg {


    public abstract class AbstractTransition : MonoBehaviour {

        [SerializeField] Worldspace worldspace;
        [SerializeField] TransformData destination;

        [Tooltip("If true, this will not load the world space.  Set this to true if you want this to teleport the player to a location in the same world.")]
        [SerializeField] bool sameWorldSpace;
        

        public void MovePlayerCharacter(PCMoving pc) {
            if(sameWorldSpace) pc.Teleport(destination);
            else WorldManagement.TransferPC(pc, worldspace, destination);
        }

    }

}
