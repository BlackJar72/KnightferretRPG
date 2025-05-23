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





    }


}

