using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg.ui {

    public class ContainerUI : InventoryPanel {

        private Container container;
        [SerializeField] GameObject top;


        public bool visible => top.activeSelf;


        protected override void OnEnable() {
            InventoryManager.closeAllInvUI += BeRemoved;
            base.OnEnable();
        }


        protected override void OnDisable() {
            InventoryManager.closeAllInvUI -= BeRemoved;
            base.OnDisable();
        }


        public void Initialize(Inventory inventory, Container container) {
            this.inventory = inventory;
            this.container = container;
            top.SetActive(true);
            Redraw();
        } 


        public void BeRemoved() {
            top.SetActive(false);
        }
        


    }


}
