using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {

    public class SpellEquiptSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {

        [SerializeField] Image icon;
        [SerializeField] bool belongsToPC;

        [HideInInspector] public Spell currentSpell;


        protected virtual void Start() {
            if(belongsToPC) InventoryManagement.HotbarActivatedEvent += RespondToHotbar;
        }


        void OnDestroy() {        
            InventoryManagement.HotbarActivatedEvent -= RespondToHotbar;
        }


        public void OnDrop(PointerEventData eventData) {            
            GameObject other = eventData.pointerDrag;
            SpellEntry spellEntry = other.GetComponent<SpellEntry>();
            if(spellEntry != null) EquiptSpell(spellEntry);
        }


        public void EquiptSpell(SpellEntry spellEntry) {
                currentSpell = spellEntry.Spell;
                icon.sprite = spellEntry.Icon.sprite;
                icon.gameObject.SetActive(true);
            }


        public void OnPointerEnter(PointerEventData eventData) {
            if(currentSpell != null) GameManager.Instance.UIManager.ShowSpellToolTip(currentSpell);
        }


        public void OnPointerExit(PointerEventData eventData) {
            GameManager.Instance.UIManager.HideItemToolTip();
        }


        public void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                GameManager.Instance.UIManager.HideItemToolTip();
                GameManager.Instance.UIManager.HideItemStackManipulator();
                currentSpell = null;
                icon.gameObject.SetActive(false);
            } 
        }


        public void RespondToHotbar(SlotData slot) {
            if(slot.inventory == InvType.SPELLS) {
                Spell otherSpell = EntityManagement.playerCharacter.Spells.spells[slot.invSlot];
                if(otherSpell == currentSpell) {
                    currentSpell = null;
                    icon.gameObject.SetActive(false);
                } else {
                    currentSpell = otherSpell;
                    icon.sprite = otherSpell.Icon;
                    icon.gameObject.SetActive(true);
                }
            }
        }


    }

}
