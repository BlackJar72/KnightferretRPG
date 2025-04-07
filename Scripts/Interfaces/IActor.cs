using UnityEngine;

namespace kfutils.rpg {

    public interface IActor {

        public void EquiptItem(ItemStack item);
        public void UnequiptItem(ItemStack item);
        public void UnequiptItem(EEquiptSlot slot);


    }


}
