using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



namespace kfutils.rpg.ui {


    public class HotbarSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler {

        
        [SerializeField] HotbarUI hotBar;
        [SerializeField] TMP_Text numberText;
        [SerializeField] int slotNumber;
        [SerializeField] Sprite unselectedImage;
        [SerializeField] Sprite selectedImage;

        public Image icon;

        private Image image;


        private void Awake()
        {
            image = GetComponent<Image>();
        }


        public void Redraw()
        {
            SlotData slotData = hotBar.GetSlot(slotNumber);
            icon.gameObject.SetActive(slotData.filled);
            if (slotData.inventory == InvType.SPELLS)
            {                
                PCActing pc = EntityManagement.playerCharacter;
                Spell spell = pc.Spells.GetItemInSlot(slotData.invSlot);
                if (spell == pc.EquiptSpell.CurrentSpell) image.sprite = selectedImage;
                else image.sprite = unselectedImage;
            }
            else if (slotData.inventory == InvType.EQUIPT) image.sprite = selectedImage;
            else image.sprite = unselectedImage;
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
                        image.sprite = unselectedImage;
                        break;
                    case InvType.EQUIPT:
                        ItemStack stack = EntityManagement.playerCharacter.Inventory.Equipt.GetItemInSlot(sd.invSlot);
                        if((stack == null) || (stack.item == null)) break; // FIXME: I don't think this will be needed if underlying bug is fixed
                        icon.sprite = stack.item.Icon;
                        image.sprite = selectedImage;
                        break;
                    case InvType.SPELLS:
                        PCActing pc = EntityManagement.playerCharacter;
                        Spell spell = pc.Spells.GetItemInSlot(sd.invSlot);
                        icon.sprite = spell.Icon;
                        if (spell == pc.EquiptSpell.CurrentSpell) image.sprite = selectedImage;
                        else image.sprite = unselectedImage;
                        break;
                    default:
                        image.sprite = unselectedImage;
                        icon.gameObject.SetActive(false);
                        break;
                }
            }
            else image.sprite = unselectedImage;
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
                PCActing pc = EntityManagement.playerCharacter;
                slot.inventory = InvType.SPELLS;
                slot.invSlot = pc.Spells.GetIndexOfSpell(spellEntry.Spell);
                slot.filled = true;
                icon.sprite = spellEntry.Icon.sprite;
                icon.gameObject.SetActive(true);
                hotBar.SlotData.CleanUpDuplicates(slotNumber, slot); 
                InventoryManagement.SigalHotbarUpdate();
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
