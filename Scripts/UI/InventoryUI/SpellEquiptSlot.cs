using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {

    public class SpellEquiptSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {

        [SerializeField] Image icon;

        [HideInInspector] public Spell currentSpell;


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


    }

}
