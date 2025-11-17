using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(fileName = "InitialPCData", menuName = "KF-RPG/World/Initial PC Data", order = -64)]
    public class InitialPCData : ScriptableObject
    {
        [Tooltip("Where the player starts out, both including position and rotation (and possibly even scale)")]
        [SerializeField] TransformData startingPosition;
        [Tooltip("Items that all player characters start the game with (not based on character creation)")]
        [SerializeField] ItemStack.ProtoStack[] startingItems;
        [Tooltip("Spells that all player characters start the game with (not based on character creation)")]
        [SerializeField] Spell[] startingSpells; 


        public void SetInitialLocation(PCMoving pc)
        {
            pc.Teleport(startingPosition);
        }


        public void AddGearToInventory(PlayerInventory inventory)
        {
            foreach (ItemStack.ProtoStack stack in startingItems)
            {
                ItemStack item = stack.MakeStack();
                ItemManagement.AddItem(new ItemData(item));
                if (stack.equipt)
                {
                    if (!inventory.Equipt.AddItemNoSlot(item)) inventory.AddToFirstEmptySlot(stack.MakeStack());
                }
                else inventory.AddToFirstEmptySlot(stack.MakeStack());
            }            
        } 


        public void AddSpellsToBook(Spellbook spellbook)
        {
            foreach (Spell spell in startingSpells) spellbook.AddToFirstEmptySlot(spell);
        }  



    }

}

