using UnityEngine;
using kfutils.rpg.ui;
using System;


namespace kfutils.rpg {

    public class CharacterInventory : Inventory {

        [SerializeField] protected EquiptmentSlots equipt;


        public EquiptmentSlots Equipt => equipt;


        void Awake() {
            equipt.mainInventory = this;
        }





    }


}

