using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace kfutils.rpg.ui {

    [System.Serializable]
    public enum InventoryType
    {
        INVALID = -1,
        EMPTY = 0,
        MAIN = 1,
        EQUIPT = 2,
        SPELL = 3
    }


    [System.Serializable]
    public struct HotbarRefData
    {
        public int slot;
        public InventoryType inventory;
        public HotbarRefData(HotbarReference reference)
        {
            if(reference.Item == null)
            {
                slot = -1;
                inventory = InventoryType.EMPTY;
            }
            else
            {
                slot = reference.Item.Slot;
                // This a new type to hold a known Spell is needed, like ItemStack is for Item.
                if(reference is Spell) inventory = InventoryType.SPELL; // FIXME!
                else inventory = reference.InventoryPanel.GetInventoryType(reference.Item);
            }
        }
    }



    [RequireComponent(typeof(Image))][System.Serializable]
    public class HotbarReference : MonoBehaviour, IInvReference, 
                                   IDropHandler, IPointerClickHandler //, IBeginDragHandler, IDragHandler, IEndDragHandler 
    {
        [SerializeField] Sprite unselectedImage;
        [SerializeField] Sprite selectedImage;
        [SerializeField] Image icon;
        [SerializeField] InventoryPanel inventoryPanel;
        [SerializeField] SpellEquiptSlot spellSlot;

        private IInvReferant referant;
        private Image image;
        private bool equipped = false;

        public IInvReferant Item => referant;
        public bool Equipped => equipped;
        public InventoryPanel InventoryPanel => inventoryPanel;


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
            image.sprite = unselectedImage;;
        }


        public void SetReferant(IInvReferant referant)
        {
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
            IInvReferant otherItem = other.referant;
            IInvReferant thisItem = referant;
            ClearReferant();
            other.ClearReferant();
            other.SetReferant(thisItem);
            SetReferant(otherItem);
        }


        public void Activate()
        {
            if(referant is ItemStack stack) inventoryPanel.RespondToHotbar(stack);
        }


        public bool HoldsReferant(IInvReferant referant)
        {
            return this.referant == referant;
        }


        public void SetSelected(bool selected)
        {
            equipped = selected;
            if(selected) image.sprite = selectedImage;
            else image.sprite = unselectedImage;
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