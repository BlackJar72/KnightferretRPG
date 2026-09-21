using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {


    public class EquiptReference : MonoBehaviour, IInvReference, 
                                   IDropHandler, IPointerClickHandler //, IBeginDragHandler, IDragHandler, IEndDragHandler 
    {
        [SerializeField] Image icon;
        [SerializeField] EquiptmentPanel equiptPanel;

        private IInvReferant referant;

        public IInvReferant Item => referant;


        // // Start is called once before the first execution of Update after the MonoBehaviour is created
        // void Start()
        // {
        //    
        // }
        //
        // // Update is called once per frame
        // void Update()
        // {
        //    
        // }
        


#region IInvReference
        public void OnReferantRemoved(IInvReferant referant)
        {
            referant = null;
            icon.gameObject.SetActive(false);
            // TODO: Unequipped item!
        }


        public void SetReferant(IInvReferant referant)
        {
            this.referant = referant;
            referant.AddReference(this);
            icon.sprite = referant.Icon;
            icon.gameObject.SetActive(true);
            // TODO: Equipped item!
        }


        public void ClearReferant()
        {
            if(referant != null) OnReferantRemoved(referant);
            referant = null;
            // TODO: Unequipped item!
        }


#endregion
#region Pointer Device (Mouse) Code


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
                if((invslot.item != referant) && invslot.inventory.OwnedByPC) {
                    ClearReferant();
                    SetReferant(invslot.item);
                }
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            ClearReferant();
        }


#endregion


    }


}