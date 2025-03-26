using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace kfutils.rpg.ui {


    /// <summary>
    /// This is meant to be added to the icon image withint the inventory slots to fascilitate 
    /// dragging.
    /// 
    /// The plan is simply to drag the image, but to trigger switching on drop.
    /// 
    /// To do this, it will check if the new parent is the same as the is the same as the old (if so
    /// do nothing).  If not it must check to see if they are the same item type and if the item is 
    /// stackable, and if so combine the stacks.  If they are different or not stackable, then call 
    /// swap.
    /// </summary>
    public class InventoryItem : MonoBehaviour, IPointerDownHandler {

        private Image image;

        public Image Icon { get => image; }


        void Awake() {
            image = GetComponent<Image>();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Mouse Down");
        }


    }


}