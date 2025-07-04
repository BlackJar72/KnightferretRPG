using UnityEngine;


namespace kfutils.rpg.ui {

    public class ContainerUI : InventoryPanel {

        private Container container;
        [SerializeField] GameObject top;


        public bool visible => top.activeSelf;


        public void Initialize(Inventory inventory, Container container) {
            InventoryManagement.currentContainerInventory = inventory;
            this.inventory = inventory;
            this.container = container;
            top.SetActive(true);
            Redraw();
        } 


        protected override void OnEnable() {
            base.OnEnable();
        }


        protected override void OnDisable() {
            base.OnDisable();
        }
        


    }


}
