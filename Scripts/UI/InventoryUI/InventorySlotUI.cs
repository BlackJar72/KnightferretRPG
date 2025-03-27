using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.GraphView;

namespace kfutils.rpg.ui {

    public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

        
        [SerializeField] public IInventory inventory;
        [SerializeField] TMP_Text numberText;
        
        [Tooltip("Which slot type this is; this should only be set to one value, though items can have more than one.")]
        [SerializeField] protected EEquiptSlot slotType;
        public EEquiptSlot SlotType { get => slotType; }

        public Image icon;
        public int slotNumber;
        
        public ItemStack item;

        private RectTransform iconTransfrom;
        


        public virtual void SwapWith(InventorySlotUI other) {
            if(!CanSwapSlotTypes(other)) return;
            if((other.item.item == item.item) && item.item.IsStackable) {
                item.stackSize += other.item.stackSize;
                other.inventory.RemoveItem(other.item);
            } else {
                item.slot = other.slotNumber;
                other.item.slot = slotNumber;
                if(other.inventory != inventory) {
                    other.inventory.AddItemToSlot(item.slot, item);
                    inventory.AddItemToSlot(other.item.slot, other.item);
                    inventory.RemoveItem(item);
                    other.inventory.RemoveItem(other.item);
                } 
            }
            inventory.SignalUpdate();           
        }


        protected bool CanSwapSlotTypes(InventorySlotUI other) {
            bool result = (item.item == null) || ((item.item.EquiptType & other.slotType) > 0);
            result = result && ((other.item.item == null) || ((other.item.item.EquiptType & slotType) > 0));
            return result;
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


        public void ShowText(bool visible) {
            numberText.gameObject.SetActive(visible);
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
