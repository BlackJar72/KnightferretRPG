using UnityEngine;
using kfutils.rpg;
using UnityEngine.EventSystems;


namespace rpg.verslika {


    public class AvatarTopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject childPanel;


        public void Start()
        {
            childPanel.SetActive(false);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            childPanel.SetActive(true);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
                if (eventData.fullyExited) childPanel.SetActive(false);
        }


        public void SetChildPanel(GameObject newChild)
        {
            childPanel = newChild;
        }

    }

}