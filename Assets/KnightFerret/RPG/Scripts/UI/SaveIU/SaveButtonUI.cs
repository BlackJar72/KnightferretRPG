using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui
{


    public class SaveButtonUI : MonoBehaviour
    {

        [SerializeField] protected TMP_InputField textField;
        [SerializeField] protected Image textAreaButton;
        [SerializeField] protected Image background;
        [SerializeField] protected Color unselected;
        [SerializeField] protected Color selected;
        [SerializeField] protected Color normalTextColor;
        [SerializeField] protected Color editTextColor;
        protected SaveLoadUI saveLoadUI;
        protected bool isSelected = false;


        public void SetText(string text) => textField.text = text;
        public void SetOwner(SaveLoadUI owner) => saveLoadUI = owner;


        public virtual void BeClicked()
        {
            saveLoadUI.SelectAsSave(this);
        }


        public void SetSelected(bool beSelected)
        {
            isSelected = beSelected;
            if (isSelected) background.color = selected;
            else background.color = unselected;
        }


        public string GetText() => textField.text;


        public bool SameButDifferent(SaveButtonUI other)
        {
            return (this != other) && other.textField.text.Equals(textField.text);
        }


        public void SetAsNewSave()
        {
            textField.readOnly = false;
            textField.textComponent.color = editTextColor;
            textAreaButton.raycastTarget = false;
            textField.text = EntityManagement.playerCharacter.GetPersonalName() + " - " + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            EventSystem.current.SetSelectedGameObject(textField.gameObject, null);
            textField.OnPointerClick(new PointerEventData(EventSystem.current));
        }


        public void SolidfyNewSave()
        {
            textField.readOnly = true;
            textField.textComponent.color = normalTextColor;
            textAreaButton.raycastTarget = true;
            EventSystem.current.SetSelectedGameObject(textField.gameObject, null);
            textField.OnPointerClick(new PointerEventData(EventSystem.current));
            saveLoadUI.SolidifyNewSaveButton(this);
        }




    }


}