using UnityEngine;
using UnityEngine.SceneManagement;


namespace kfutils.rpg {


    public abstract class AbstractTransition : MonoBehaviour {

        [SerializeField] Worldspace worldspace;
        [SerializeField] string destinationID;

        [Tooltip("If true, this will not load the world space.  Set this to true if you want this to teleport the player to a location in the same world.")]
        [SerializeField] bool sameWorldSpace;

        public Worldspace World => worldspace;
        public string DestinationID => destinationID;
        

        public void MovePlayerCharacter(PCMoving pc) {
            if(sameWorldSpace) MovePlayerCharacterInWorld(pc); //pc.Teleport(destination);
            else WorldManagement.TransferPC(pc, worldspace, this);
        }


        private void MovePlayerCharacterInWorld(PCMoving pc) {
            TeleportMarker marker = WorldManagement.teleportMarkers[destinationID];
            if(marker != null) pc.Teleport(marker.transform);
        }

    }

}
