using System;
using UnityEngine;


namespace kfutils.rpg {


    /// <summary>
    /// An intermediary container for an IEquipable that handles specifics that may vary 
    /// between IEquipable types as well as some management relating to equipping and 
    /// dealing with the underlying equipable object.
    /// 
    /// Examples of classes that should be IEquipable and thus used with this class include 
    /// item stacks and spells.  Likely there will be two main categories, items (held in 
    /// inventories, dropable, buy/sellable, storable) and broadly abilities (e.g., spells, 
    /// generally something representing knowledge or a train of the character, thus not 
    /// stored in inventories, not dropable, not stackable, etc.).  A stackable ability (e.g, 
    /// Vancian spells) could hypothetically be another types, but not planned for the game 
    /// being developed.
    /// 
    /// The idea is to make the use of equipt slots and hot bars simpler and more flexible by 
    /// allowing equipping without moving the item out of the inventory.  This would, if 
    /// successful, eliminate some of the convoluted record keeping and manipulation that is 
    /// currently involved in equiping items (especially with keeping the hotbar sycned), 
    /// as well as its various corner-cases.  In particular I hope this will fix some of the 
    /// problems the arrise with two-handed items on hotbars in the old system.  Done right, 
    /// it should also eliminate the need to relegate spells to a special spell slot, thus 
    /// allowing spell to be equipped to the hands if desired. 
    /// </summary>
    [System.Serializable]
    public class EquiptHolder : MonoBehaviour 
    {
        [SerializeField] IEquipable held;


        public IEquipable Held => held;

        public bool IsEquipped { get => held.IsEquipped; set => held.IsEquipped = value; }
        public bool IsOnHotbar { get => held.IsOnHotbar; set => held.IsOnHotbar = value; }
        public ItemPrototype item { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int stackSize { get => held.StackSize; set => held.StackSize = value; }
        public int slot { get => held.Slot; set => held.Slot = value; }
        public int Difficulty => held.Difficulty;


        public void Equip(IEquipable equipable)
        {
            UnEquip();
            held = equipable;
            held.IsEquipped = true;
        }


        public void UnEquip()
        {
            held.IsEquipped = false;
        }



    }


}