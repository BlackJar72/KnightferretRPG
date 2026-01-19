using UnityEngine;



namespace kfutils.rpg.ui {

    public class TabbedSubpanel : MonoBehaviour
    {
        [SerializeField] string tabName;

        private TabbedPanel controller;

        public string TabName => tabName;


        public void SetController(TabbedPanel tabbedPanel)
        {
            controller = tabbedPanel;
        }





    }

}
