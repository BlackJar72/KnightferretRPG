using UnityEngine;



namespace kfutils.rpg.ui {

    public class ShowOrHide : MonoBehaviour {

        [SerializeField] bool showAtStart;
        private bool isVisible;
        public bool IsVisible { get => isVisible; }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            isVisible = showAtStart;
            gameObject.SetActive(isVisible);
        }


        private void Apply() {
            gameObject.SetActive(isVisible);
        }


        public void SetVisible() {
            isVisible = true;
            Apply();
        }


        public void SetVisible(bool show) {
            isVisible = show;
            Apply();
        }


        public void SetHidden() {
            isVisible = false;
            Apply();
        }


        public void Toggle() {
            isVisible = !isVisible;
            Apply();
        }

    }

}