using UnityEngine;



namespace kfutils.rpg
{

    public interface IActorAI
    {
        public void EquipItem(ItemStack stack);
        public void AddNewItemNPC(ItemStack stack);
        public void AddNewEquiptItem(ItemStack stack);
        public void UnequipItem(ItemStack stack);
        public void UseItem(EEquiptSlot slot);

        public AIState CurrentAIState { get; }


    }

}
