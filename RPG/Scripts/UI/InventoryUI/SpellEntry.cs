using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {
    
    public class SpellEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
                              IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        private Spell spell;
        private int slot;

        [SerializeField] GameObject spellSlot;
        [SerializeField] Image spellIcon;
        [SerializeField] TMP_Text spellName;

        public Spell Spell { get => spell; }
        public Image Icon => spellIcon;

        private Image dragged;


        public void AddSpell(Spell spell, int slot) {
            gameObject.SetActive(true);
            this.spell = spell;
            this.slot = slot;
            spellIcon.sprite = spell.Icon;
            spellName.SetText(spell.Name);
        }


        #region Drag and Drop


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (spellIcon.sprite != null)
            {
                if (dragged == null)
                {
                    dragged = Instantiate<Image>(spellIcon, spellIcon.transform.parent);
                    dragged.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform);
                    dragged.raycastTarget = false;
                }
                else dragged.gameObject.SetActive(true);
                dragged.transform.position = Input.mousePosition;
                GameManager.Instance.UI.HideItemToolTip();
                GameManager.Instance.UI.HideItemStackManipulator();
            }
        }


        public void OnDrag(PointerEventData eventData) {  
            dragged.transform.position = Input.mousePosition;
        }


        public void OnEndDrag(PointerEventData eventData) {
            dragged.gameObject.SetActive(false);
        }


        public void OnPointerEnter(PointerEventData eventData) {
            if(spell != null) GameManager.Instance.UI.ShowSpellToolTip(spell);
        }


        public void OnPointerExit(PointerEventData eventData) {
            GameManager.Instance.UI.HideItemToolTip();
        }


        public void OnPointerClick(PointerEventData eventData) {
            if(eventData.button == PointerEventData.InputButton.Left) {
                EntityManagement.playerCharacter.EquiptSpell.EquiptSpell(this);
            } 
        }


        #endregion


    }

}
