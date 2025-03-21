using UnityEngine;


namespace kfutils.rpg
{

    public class DoorWrapper : MonoBehaviour, IInteractable
    {
        [SerializeField] SimpleOpener door;


        public void Use() {
            door.Activate();
        }

    }


}
