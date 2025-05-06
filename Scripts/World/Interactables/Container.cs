using UnityEngine;
using kfutils.rpg.ui;

namespace kfutils.rpg {

    public class Container : MonoBehaviour, IInteractable {

        [SerializeField] Inventory inventory;



        public void Use(GameObject from) {
            GameManager.Instance.UIManager.ToggleContainerUI(inventory, this, from);
        }


    }

}
