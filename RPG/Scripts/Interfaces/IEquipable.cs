using UnityEngine;


namespace kfutils.rpg
{

    public interface IEquipable 
    {
        public bool IsEquipped { get; set; }
        public bool IsOnHotbar { get; set; }

        // Current found in ItemStack
        public ItemPrototype item { get; set; }
        public int stackSize { get; set; }
        public int slot { get; set; }
        // Currently found in Spell
        public int Difficulty { get; }

        public void Equip();
        
    }


}

