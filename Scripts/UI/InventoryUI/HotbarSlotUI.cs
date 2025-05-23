using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace kfutils.rpg.ui {


    public class HotbarSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler {

        
        [SerializeField] public HotbarUI hotBar;
        [SerializeField] TMP_Text numberText;
        [SerializeField] int slotNumber;

        public Image icon;


        public void Redraw() {            
            icon.gameObject.SetActive(hotBar.GetSlot(slotNumber).filled);
        }


        public void RedrawForLoad(HotbarUI parent)
        {
            hotBar = parent;
            if (hotBar.GetSlot(slotNumber).filled)
            {
                SlotData sd = hotBar.GetSlot(slotNumber);
                icon.gameObject.SetActive(hotBar.GetSlot(slotNumber).filled);
                switch (sd.inventory)
                {
                    case InvType.MAIN:
                        icon.sprite = EntityManagement.playerCharacter.Inventory.GetItemInSlot(sd.invSlot).item.Icon;
                        break;
                    case InvType.EQUIPT:
                        icon.sprite = EntityManagement.playerCharacter.Inventory.Equipt.GetItemInSlot(sd.invSlot).item.Icon;
                        break;
                    case InvType.SPELLS:
                        icon.sprite = EntityManagement.playerCharacter.Spells.GetItemInSlot(sd.invSlot).Icon;
                        break;
                    default:
                        icon.gameObject.SetActive(false);
                        break;
                }
            }            
        }


        public void OnPointerClick(PointerEventData eventData) {
            if((eventData.button == PointerEventData.InputButton.Left) && (eventData.clickCount == 2)) {                
                SlotData slot = hotBar.GetSlot(slotNumber);
                slot.inventory = InvType.NONE;
                slot.invSlot = -1;
                slot.filled = false;
                hotBar.Redraw();
            } 
        }


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
            SpellEntry spellEntry = other.GetComponent<SpellEntry>();
            if(spellEntry != null) {
                SlotData slot = hotBar.GetSlot(slotNumber);
                slot.inventory = InvType.SPELLS;
                slot.invSlot = EntityManagement.playerCharacter.Spells.GetIndexOfSpell(spellEntry.Spell);
                slot.filled = true; 
                icon.sprite = spellEntry.Icon.sprite;
                icon.gameObject.SetActive(true);
                hotBar.SlotData.CleanUpDuplicates(slotNumber, slot);
                hotBar.Redraw();
                return;
            }

        }


        private void SetItemInSlot(SlotData slot, InventorySlotUI invSlot) {
                slot.invSlot = invSlot.slotNumber;
                slot.filled = true; 
                icon.sprite = invSlot.icon.sprite;
                icon.gameObject.SetActive(true);
                hotBar.SlotData.CleanUpDuplicates(slotNumber, slot);
                hotBar.Redraw();
        }


    }

}
