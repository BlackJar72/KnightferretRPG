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
            base.OnEnable();
            Register();
        }


        public virtual void Register() {
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
            if(m < 0) {
                m = money;
                InventoryManagement.StoreMoneyData(money, owner.ID);
            } else {
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

