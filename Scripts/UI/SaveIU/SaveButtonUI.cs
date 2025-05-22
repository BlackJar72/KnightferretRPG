using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg
{


    public class SaveButtonUI : MonoBehaviour
    {

        [SerializeField] protected TMP_InputField textField;
        [SerializeField] protected Image background;
        [SerializeField] protected Color unselected;
        [SerializeField] protected Color selected;
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




    }


}