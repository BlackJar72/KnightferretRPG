using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace kfutils.rpg.ui {


    public class HotbarSlotUI : MonoBehaviour, IDropHandler {

        
        [SerializeField] public HotbarUI hotBar;
        [SerializeField] TMP_Text numberText;
        [SerializeField] int slotNumber;

        public Image icon;












        public void OnDrop(PointerEventData eventData) {
            GameObject other = eventData.pointerDrag;
            EquipmentSlotUI equiptSlot = other.GetComponent<EquipmentSlotUI>();
            if((equiptSlot != null) && (equiptSlot.icon.sprite != null)) {
                SlotData slot = hotBar.GetSlot(slotNumber);
                slot.inventory = InvType.EQUIPT;
                SetItemInSlot(slot, equiptSlot);
                return;
            }
            InventorySlotUI invSlot = other.GetComponent<InventorySlotUI>();
            if((invSlot != null) && (invSlot.icon.sprite != null)) {
                SlotData slot = hotBar.GetSlot(slotNumber);
                slot.inventory = InvType.MAIN;
                SetItemInSlot(slot, invSlot);
                return;
            }

        }


        private void SetItemInSlot(SlotData slot, InventorySlotUI invSlot) {
                slot.invSlot = invSlot.slotNumber;
                slot.filled = true; 
                icon.sprite = invSlot.icon.sprite;
                icon.gameObject.SetActive(true);
                hotBar.Redraw();
        }


    }

}
