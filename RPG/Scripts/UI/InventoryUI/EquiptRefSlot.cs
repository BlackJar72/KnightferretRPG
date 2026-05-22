using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace kfutils.rpg.ui {

    public class EquiptRefSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
                                   IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {    
        private StackRef reference = new(); 

        [SerializeField] public IInventory<ItemStack> inventory;
        [SerializeField] TMP_Text numberText;
        
        [Tooltip("Which slot type this is; this should only be set to one value, though items can have more than one.")]
        [SerializeField] protected EEquiptSlotFlags slotType;
        public EEquiptSlotFlags SlotType { get => slotType; }

        public Image icon;
        public int localSlot;

        public int InvSlotNumber => reference.Item.SlotNumber;
        public IInventoryResident Resident => reference.Item;
        public ItemStack item => reference.Item as ItemStack; // Seems pointless, but this will later return an interface
        public InventoryPanel inventoryPanel;




        //*************************************************************************************************************//
        //                                         REFERRENCE INTERTAIONS                                              //
        //*************************************************************************************************************//




        //*************************************************************************************************************//
        //                                            MOUSE INTERACTIONS                                               //
        //*************************************************************************************************************//

        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }



    }


}
