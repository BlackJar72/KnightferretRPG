using UnityEngine;


namespace kfutils.rpg
{

    public interface IEquipable 
    {
        public bool IsEquipped { get; set; }
        public bool IsOnHotbar { get; set; }

        // Current found in ItemStack
        public ItemPrototype Item { get; set; }
        public int StackSize { get; set; }
        public int Slot { get; set; }
        // Currently found in Spell
        public int Difficulty { get; }

        public void Equip();
        public void UnEquip();

        public void AddToHotbar();
        public void RemoveFromHotbar();
        
    }


}

