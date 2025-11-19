using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace kfutils.rpg {


    public class StatAdjusterUI : MonoBehaviour
    {
        [SerializeField] EBaseStats stat; 
        [SerializeField] TMP_Text label;
        [SerializeField] TMP_Text valueText;
        [SerializeField] Button incrementButton;
        [SerializeField] Button decrementButton;

        private StatCreationUI parent;

        private int value;


        public void Start()
        {
                label.text = stat.ToString();
        }


        public void SetParent(StatCreationUI characterCreationUI)
        {
            parent =  characterCreationUI;
            value = parent.Stats[stat];
            valueText.text = value.ToString();
        }


        public void UpdateText()
        {
            value = parent.Stats[stat];
            valueText.text = value.ToString();
        }

        
        public void Increment()
        {
            if((value >= EntityBaseStats.MAX_SCORE) || parent.NoAdditions)  return;
            value++;
            parent.Stats[stat] = value;
            valueText.text = value.ToString();
            parent.UpdateAdditions(-1);
            if(value >= EntityBaseStats.MAX_SCORE) HideIncrementButton();
            if(value > 1) ShowDecrementButton();
        }

        
        public void Decriment()
        {
            if(value < 2) return;
            value--;
            parent.Stats[stat] = value;
            valueText.text = value.ToString();
            parent.UpdateAdditions(1);
            if(value < 2) HideDecrementButton();
            if(value < EntityBaseStats.MAX_SCORE) ShowIncrementButton();
        }


        public void ShowIncrementButton()
        {
            incrementButton.interactable = true;
            incrementButton.gameObject.SetActive(true);
        }


        public void ShowDecrementButton()
        {
            decrementButton.interactable = true;
            decrementButton.gameObject.SetActive(true);
        }


        public void HideIncrementButton()
        {
            incrementButton.interactable = false;
            incrementButton.gameObject.SetActive(false);
        }


        public void HideDecrementButton()
        {
            decrementButton.interactable = false;
            decrementButton.gameObject.SetActive(false);
        }


        public void PossiblyShowIncrementButton()
        {
            if(value < EntityBaseStats.MAX_SCORE) ShowIncrementButton();
        }






    }


}
