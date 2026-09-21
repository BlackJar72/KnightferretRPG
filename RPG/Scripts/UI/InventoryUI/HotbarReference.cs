using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {


    [RequireComponent(typeof(Image))]
    public class HotbarReference : MonoBehaviour, IInvReference, 
                                   IDropHandler, IPointerClickHandler //, IBeginDragHandler, IDragHandler, IEndDragHandler 
    {
        [SerializeField] Sprite unselectedImage;
        [SerializeField] Sprite selectedImage;
        [SerializeField] Image icon;

        private IInvReferant referant;
        private Image image;

        public IInvReferant Item => referant;


        // // Start is called once before the first execution of Update after the MonoBehaviour is created
        // void Start()
        // {

        // }

        // // Update is called once per frame
        // void Update()
        // {

        // }


        private void Awake()
        {
            image = GetComponent<Image>();
        }


        public void OnReferantRemoved(IInvReferant referant)
        {
            referant = null;
            icon.gameObject.SetActive(false);
            image.sprite = unselectedImage;
            Debug.Log("OnReferantRemoved(IInvReferant referant)");
        }


        public void SetReferant(IInvReferant referant)
        {
            Debug.Log("SetReferant(IInvReferant referant)");
            this.referant = referant;
            referant.AddReference(this);
            icon.sprite = referant.Icon;
            icon.gameObject.SetActive(true);
            // FIXME: Add add way to get if it is equipt, then change this!
            image.sprite = unselectedImage;
        }


        public void ClearReferant() => OnReferantRemoved(referant);


        public void SwapWith(HotbarReference other)
        {
            Debug.Log("SwapWith(HotbarReference other)");
            IInvReferant otherItem = other.referant;
            IInvReferant thisItem = referant;
            ClearReferant();
            other.ClearReferant();
            other.SetReferant(thisItem);
            SetReferant(otherItem);
        }




        //-----------------------------------------------------------------------------------------------------------//
        //                                POINTER DEVISE (MOUSE) HANDLING                                            //
        //-----------------------------------------------------------------------------------------------------------//
        #region Mouse / Pointer Devise Handling


/* 
        // public void OnBeginDrag(PointerEventData eventData)
        // {
        //     //Debug.Log("public void OnBeginDrag(PointerEventData eventData)");
        //     if(icon.sprite != null) {
        //         icon.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform);
        //         GameManager.Instance.UI.HideItemToolTip();
        //         GameManager.Instance.UI.HideItemStackManipulator();
        //     }
        // }


        // public void OnDrag(PointerEventData eventData)
        // {
        //     //Debug.Log("public void OnDrag(PointerEventData eventData)");
        //     icon.transform.position = Input.mousePosition;
        // }


        // public void OnEndDrag(PointerEventData eventData)
        // {
        //     //Debug.Log("public void OnEndDrag(PointerEventData eventData)");
        //     icon.transform.SetParent(transform);
        //     icon.transform.localPosition = Vector2.zero;  
        //     GameObject under = eventData.pointerCurrentRaycast.gameObject;
        //     if((under == null) || !under.TryGetComponent<HotbarReference>(out HotbarReference other))
        //     ClearReferant();
        // } 
*/


        public void OnDrop(PointerEventData eventData)
        {
            GameObject other = eventData.pointerDrag;
            if(other == gameObject) return;
            if(other.TryGetComponent<InventorySlotUI>(out InventorySlotUI invslot))
            {
                if(invslot.inventory.OwnedByPC) {
                    ClearReferant();
                    SetReferant(invslot.item);
                }
            } 
            else if (other.TryGetComponent<HotbarReference>(out HotbarReference otherRef))
            {
                SwapWith(otherRef);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            ClearReferant();
        }




        #endregion


    }


}