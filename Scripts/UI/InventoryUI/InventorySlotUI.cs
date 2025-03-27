using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.GraphView;

namespace kfutils.rpg.ui {

    public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

        
        public Image icon;
        public Inventory inventory;
        public int slotNumber;
        [SerializeField] TMP_Text numberText;
        
        public ItemStack item;

        private RectTransform iconTransfrom;


        public virtual void SwapWith(InventorySlotUI other) {
            bool successful = false;
            if((other.item.item == null) || (other.icon.sprite == null)) {
                item.slot = other.slotNumber;
                if(other.inventory != inventory) {
                    successful = other.inventory.AddItemToSlot(slotNumber, item);
                    if(successful) {
                        inventory.RemoveItem(item);
                        inventory = other.inventory;
                    } else {
                        item.slot = slotNumber;
                    }
                }
            } else {
                if((other.item.item == item.item) && item.item.IsStackable) {
                    item.stackSize += other.item.stackSize;
                    inventory.RemoveItem(other.item);
                    successful = true;
                } else {
                    item.slot = other.slotNumber;
                    other.item.slot = slotNumber;
                    if(other.inventory != inventory) {
                        Inventory originalInv = inventory;
                        successful = other.inventory.AddItemToSlot(slotNumber, item);
                        if(successful) successful = successful && originalInv.AddItemToSlot(other.slotNumber, other.item);
                        if(successful) {
                            inventory.RemoveItem(item);
                            other.inventory.RemoveItem(item);
                            inventory = other.inventory;
                            other.inventory = originalInv;
                        } else {
                            item.slot = slotNumber;
                            other.item.slot = other.slotNumber;
                        }
                    } 
                }
            }
            inventory.SignalUpdate();           
        }


        

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int number) {
            number = Mathf.Min(number, item.stackSize);
            item.stackSize -= number;
            if(item.stackSize < 1) {
                inventory.RemoveItem(item);
                icon.sprite = null;
                icon.enabled = false;
            } else if(item.item.IsStackable && (numberText != null)) {
                numberText.SetText(item.stackSize.ToString());
            } 
        }


        public void HideText() {
            numberText.gameObject.SetActive(false);
        }


        public void ShowText() {
            numberText.gameObject.SetActive(true);
        }


        public void SetText(int number) {
            numberText.SetText(number.ToString());
        }

#region Drag and Drop


        public void OnPointerDown(PointerEventData eventData) {            
            //Debug.Log("Mouse Down at " + slotNumber);
        }


        public void OnBeginDrag(PointerEventData eventData) {
            //Debug.Log("Starting Drag");
            if(icon.sprite != null) {
                icon.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform);
            }
        }


        public void OnEndDrag(PointerEventData eventData) {        
            // Debug.Log("Drag Ended.");  
            icon.transform.SetParent(transform);
            icon.transform.localPosition = Vector2.zero;  
        }


        public void OnDrag(PointerEventData eventData) {
            //Debug.Log("Dragging...");
            icon.transform.position = Input.mousePosition;
        }


        public void OnDrop(PointerEventData eventData) {
            GameObject other = eventData.pointerDrag;
            if(other == gameObject) {
                //Debug.Log("Back home!");
            } else {
                InventorySlotUI otherSlot = other.GetComponent<InventorySlotUI>();
                if((otherSlot != null) && (otherSlot.icon.sprite != null)) SwapWith(otherSlot);
            }
        }


        #endregion





    }

}
