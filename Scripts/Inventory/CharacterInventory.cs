using UnityEngine;
using kfutils.rpg.ui;
using System;


namespace kfutils.rpg {

    public class CharacterInventory : Inventory {

        [SerializeField] protected EquiptmentSlots equipt;
        
        protected IActor owner;


        public EquiptmentSlots Equipt => equipt;
        public IActor Owner => owner;
        
        [SerializeField] protected Money money;
        public Money MoneyHeld => money;


        public void SetOwner(IActor actor)
        {
            // Do not allow this to be changed once set
            if (owner == null) owner = actor;
        }


        void Awake() {
            equipt.mainInventory = this;
        }


        public override void OnEnable()
        {
            if (equipt == null) FixEquipt();
            equipt.mainInventory = this;
            InventoryData data = InventoryManagement.GetInventoryData(ID);
            if (data == null)
            {
                data = new(this);
                InventoryManagement.StoreInventoryData(data);
                foreach (ItemStack.ProtoStack stack in startingItems)
                {
                    ItemStack item = stack.MakeStack();
                    ItemManagement.AddItem(new ItemData(item));
                    if (stack.equipt)
                    {
                        if (!equipt.AddItemNoSlot(item)) AddToFirstEmptySlot(stack.MakeStack());
                    }
                    else AddToFirstEmptySlot(stack.MakeStack());
                }
            }
            else
            {
                inventory = data.inventory;
                weight = data.weight;
            }
            Register();
            initialized = true;
        }


        public void FixEquipt()
        {
            equipt = InventoryManagement.GetEquiptData(ID);
            Debug.Log(equipt);
            equipt.mainInventory = this;
        }


        public virtual void Register()
        {
            EquiptmentSlots data = InventoryManagement.GetEquiptData(owner.ID);
            if (data == null)
            {
                data = equipt;
                InventoryManagement.StoreEquiptData(data);
            }
            else
            {
                equipt.CopyInto(data);
                equipt.SignalUpdate();
            }
            Money m = InventoryManagement.GetMoneyData(owner.ID);
            if (m < 0)
            {
                m = money;
                InventoryManagement.StoreMoneyData(money, owner.ID);
            }
            else
            {
                money = m;
            }
        }


        public override EquiptmentSlots GetEquiptmentSlots()
        {
            return equipt;
        }


        public override void PreSaveEquipt()
        {
            equipt.PreSave();
        }





    }


}

