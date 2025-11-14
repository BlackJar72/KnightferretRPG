using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {

    public class SpellEquiptSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {

        [SerializeField] Image icon;
        [SerializeField] bool belongsToPC;

        [SerializeField] public Spell currentSpell;

        public Spell CurrentSpell => currentSpell;


        protected virtual void Start()
        {
            if (belongsToPC) InitForPC();
        }
        

        public void InitForPC()
        {
            InventoryManagement.AssignHotbarActivated(RespondToHotbar);
        }


        void OnDestroy() {        
            InventoryManagement.HotbarActivatedEvent -= RespondToHotbar;
        }


        public void OnDrop(PointerEventData eventData) {            
            GameObject other = eventData.pointerDrag;
            SpellEntry spellEntry = other.GetComponent<SpellEntry>();
            if(spellEntry != null) EquiptSpell(spellEntry);
        }


        public void EquiptSpell(SpellEntry spellEntry) 
        {
            currentSpell = spellEntry.Spell;
            icon.sprite = spellEntry.Icon.sprite;
            icon.gameObject.SetActive(true);
            InventoryManagement.SigalHotbarUpdate();
        }


        public void OnPointerEnter(PointerEventData eventData) {
            if(currentSpell != null) GameManager.Instance.UI.ShowSpellToolTip(currentSpell);
        }


        public void OnPointerExit(PointerEventData eventData) {
            GameManager.Instance.UI.HideItemToolTip();
        }


        public void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.HideItemStackManipulator();
                currentSpell = null;
                icon.gameObject.SetActive(false);
                InventoryManagement.SigalHotbarUpdate();
            } 
        }


        public void RespondToHotbar(SlotData slot) {
            if (slot.inventory == InvType.SPELLS)
            {
                Spell otherSpell = EntityManagement.playerCharacter.Spells.Spells[slot.invSlot];
                if (otherSpell == currentSpell)
                {
                    currentSpell = null;
                    icon.gameObject.SetActive(false);
                }
                else
                {
                    currentSpell = otherSpell;
                    icon.sprite = otherSpell.Icon;
                    icon.gameObject.SetActive(true);
                }
                InventoryManagement.SigalHotbarUpdate();
            }
        }


    }

}
