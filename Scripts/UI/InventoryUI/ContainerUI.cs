using UnityEngine;


namespace kfutils.rpg.ui {

    public class ContainerUI : InventoryPanel {

        private Container container;
        [SerializeField] GameObject top;


        public bool visible => top.activeSelf;


        public void Initialize(Inventory inventory, Container container) {
            InventoryManager.currentContainerInventory = inventory;
            this.inventory = inventory;
            this.container = container;
            top.SetActive(true);
            Redraw();
        } 


        protected virtual void OnEnable() {
            base.OnEnable();
        }


        protected virtual void OnDisable() {
            base.OnDisable();
        }
        


    }


}
