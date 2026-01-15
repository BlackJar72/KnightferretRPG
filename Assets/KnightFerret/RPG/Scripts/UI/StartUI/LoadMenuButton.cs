using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui
{


    public class LoadMenuButton : MonoBehaviour
    {

        [SerializeField] protected TMP_InputField textField;
        [SerializeField] protected Image textAreaButton;
        [SerializeField] protected Image background;
        [SerializeField] protected Color unselected;
        [SerializeField] protected Color selected;
        [SerializeField] protected Color normalTextColor;
        [SerializeField] protected Color editTextColor;
        protected LoadMenu loadMenu;
        protected bool isSelected = false;


        public void SetText(string text) => textField.text = text;
        public void SetOwner(LoadMenu owner) => loadMenu = owner;


        public virtual void BeClicked()
        {
            loadMenu.SelectAsSave(this);
        }


        public void SetSelected(bool beSelected)
        {
            isSelected = beSelected;
            if (isSelected) background.color = selected;
            else background.color = unselected;
        }


        public string GetText() => textField.text;


        public bool SameButDifferent(LoadMenuButton other)
        {
            return (this != other) && other.textField.text.Equals(textField.text);
        }



    }

}
