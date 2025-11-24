using TMPro;
using UnityEngine;
using UnityEngine.UIElements;



namespace kfutils.rpg.ui {

    [RequireComponent(typeof(Button))]
    public class TabButton : MonoBehaviour
    {
        [SerializeField] TMP_Text buttonText;

        private TabbedPanel controller;

        public TMP_Text ButtonText => buttonText;


        public void SetController(TabbedPanel panel)
        {
            controller = panel;
        } 


        public void BeClicked()
        {
            controller.ShowSubpanel(buttonText.text);
        }
    }

}
