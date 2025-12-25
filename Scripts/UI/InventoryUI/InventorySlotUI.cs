using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace kfutils.rpg.ui {

    public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, 
                                   IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        
        [SerializeField] public IInventory<ItemStack> inventory;
        [SerializeField] TMP_Text numberText;
        
        [Tooltip("Which slot type this is; this should only be set to one value, though items can have more than one.")]
        [SerializeField] protected EEquiptSlotFlags slotType;
        public EEquiptSlotFlags SlotType { get => slotType; }

        public Image icon;
        public int slotNumber;
        
        public ItemStack item;
        public InventoryPanel inventoryPanel;
        


        public virtual bool SwapWith(InventorySlotUI other) {
            if(!CanSwapSlotTypes(other)) return false;
            if(inventory.OwnedByPC) InventoryManagement.SignalSlotsSwapped(SlotDataFromSlot(), other.SlotDataFromSlot());
            else InventoryManagement.SignalSlotEmptied(other.SlotDataFromSlot());
            //if((other == null) || (other.item == null) || (item == null)) return false;
            if((other.item.item == item.item) && item.item.IsStackable) {
                item.stackSize += other.item.stackSize;
                other.inventory.RemoveItem(other.item);
            } else {
                item.slot = other.slotNumber;
                other.item.slot = slotNumber;
                if(other.inventory != inventory) {
                    ItemStack localItem = item;
                    ItemStack otherItem = other.item;
                    ItemStack otherHand = null;
                    if(NeedsTwoHands(other.item.item.EquiptType) && (inventory is EquiptmentSlots)) {
                        EquiptmentSlots eqiptSlots = inventory as EquiptmentSlots;
                        otherHand = eqiptSlots.GetOtherHandItem(slotNumber);
                    }
                    inventory.RemoveItem(item);
                    other.inventory.RemoveItem(other.item);
                    other.inventory.AddItemToSlot(item.slot, localItem);
                    inventory.AddItemToSlot(other.item.slot, otherItem);
                    if((otherHand != null) && (otherHand.item != null) && !NeedsTwoHands(otherHand.item.EquiptType)) {
                        other.inventory.AddToFirstEmptySlot(otherHand);
                    }
                    GameManager.Instance.UI.HideItemToolTip();
                } 
            }
            inventory.SignalUpdate(); 
            InventoryManagement.SigalHotbarUpdate();
            return true;          
        }


        private bool NeedsTwoHands(EEquiptSlot equiptType)
        {
            return (equiptType == EEquiptSlot.HANDS) || (equiptType == EEquiptSlot.BOW);
        }


        protected bool CanSwapSlotTypes(InventorySlotUI other) {
            bool result = (item.item == null) || item.item.FitsFlags(other.slotType);
            result = result && ((other.item.item == null) || other.item.item.FitsFlags(slotType));
            return result;
        }


        public virtual SlotData SlotDataFromSlot() {
            if(item.dummy) return null;
            SlotData result = new SlotData();
            result.inventory = InvType.MAIN;
            result.invSlot = slotNumber;
            result.filled = (item != null) && (item.item != null);
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
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.HideItemStackManipulator();
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
            if(other != gameObject) {
                InventorySlotUI otherSlot = other.GetComponent<InventorySlotUI>();
                if((otherSlot != null) && (otherSlot.icon.sprite != null)) SwapWith(otherSlot);
            }
        }


        public void OnPointerEnter(PointerEventData eventData) {
            if((item != null) && (item.item != null)) {
                GameManager.Instance.UI.ShowItemToolTip(item.item);
            }
        }


        public void OnPointerExit(PointerEventData eventData) {
            GameManager.Instance.UI.HideItemToolTip();
        }


        public virtual void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.HideItemStackManipulator();
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (inventory == InventoryManagement.currentContainerInventory) {
                    EntityManagement.playerCharacter.AddToMainInventory(item);
                    inventory.RemoveItem(item);
                } else if(eventData.clickCount == 2) {
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                    if (GameManager.Instance.UI.IsContainerUIVisible) {
                        InventoryManagement.currentContainerInventory.AddToFirstEmptySlot(item);
                        inventory.RemoveItem(item);
                    } else {
                        EquipmentSlotUI destination = inventoryPanel.EquiptPanel.GetSlotForEquipt(item.item);
                        GameManager.Instance.UI.HideItemToolTip();
                        if(destination != null) {
                            destination.SwapWith(this);
                        }
                    }
                }
            } else if(eventData.button == PointerEventData.InputButton.Right) {
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.ShowItemStackManipulator(this);
            }
        }


        #endregion


        public virtual void EquipItem() {
            EquipmentSlotUI destination = inventoryPanel.EquiptPanel.GetSlotForEquipt(item.item);
            if(destination != null) {
                destination.SwapWith(this);
            }
        }



    }

}
