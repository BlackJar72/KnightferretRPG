using UnityEngine;
using UnityEngine.EventSystems;

namespace kfutils.rpg.ui {

    public class EquipmentSlotUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, 
                                   IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {

        public EquiptmentPanel equiptPanel;

// Must re-write (in the new, unimplemented methods I think)
/*
        public override bool SwapWith(InventorySlotUI other) {
            if(base.SwapWith(other)) {
                if((inventory == other.inventory) && (item.slot != slotNumber)) {
                    if((other.item.item == item.item) && item.item.IsStackable) {
                        item.stackSize += other.item.stackSize;
                        other.inventory.RemoveItem(other.item);
                    } else {
                        item.slot = other.slotNumber;
                        other.item.slot = slotNumber;
                        ItemStack localStack = item.Copy();
                        ItemStack otherStack = other.item.Copy();
                        inventory.RemoveAllFromSlot(slotNumber);
                        inventory.RemoveAllFromSlot(other.slotNumber);
                        inventory.AddItemToSlot(other.slotNumber, localStack);
                        inventory.AddItemToSlot(slotNumber, otherStack);
                    }
                    inventory.SignalUpdate();
                }
                return true;
            }
            return false;
        }


        public override SlotData SlotDataFromSlot() {
            SlotData result = new SlotData();
            result.inventory = InvType.EQUIPT;
            result.invSlot = slotNumber;
            result.filled = (item != null) && (item.item != null);
            return result;
        }


        public override void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.HideItemStackManipulator();
                if(eventData.clickCount == 2) {
                    InventorySlotUI other = equiptPanel.MainInventoryPanel.GetFirstEmptrySlot();
                    if(other != null) {
                        GameManager.Instance.UI.HideItemToolTip();
                        if(other != null) {
                            other.SwapWith(this);
                        }
                    }

                }
            } 
        }


        public override void EquipItem() {
            InventorySlotUI other = equiptPanel.MainInventoryPanel.GetFirstEmptrySlot();
            if(other != null) {
                if(other != null) {
                    other.SwapWith(this);
                }
            }

        }

*/

#region Drag and Drop


        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }

}
