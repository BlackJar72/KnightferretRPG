using TMPro;
using UnityEngine;


namespace kfutils.rpg {


    public class StatTooltip : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] RectTransform rectTransform;


        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }


        public void Show(string textToShow, RectTransform otherTransform)
        {
            text.text = textToShow;
            Vector3 newPos = new Vector3(rectTransform.position.x, otherTransform.position.y, 0);
            rectTransform.position = newPos;
            gameObject.SetActive(true);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }


    }

}
