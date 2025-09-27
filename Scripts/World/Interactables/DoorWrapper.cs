using UnityEngine;


namespace kfutils.rpg
{

    public class DoorWrapper : MonoBehaviour, IInteractable, IWorldBool 
    {
        [SerializeField] SimpleOpener door;
        [SerializeField] string id;

        public bool GetWorldState => false; // TODO

        public string ID => id;

        public void Use(GameObject other) {
            door.Activate();
        }

    }


}
