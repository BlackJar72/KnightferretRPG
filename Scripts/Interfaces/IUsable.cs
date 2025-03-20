using UnityEngine;


namespace kfutils.rpg {

    /// <summary>
    /// Used for items that can be used from the inventory. 
    /// For weapons, this should call its relevant attack method.
    /// 
    /// (For objects that can be used in the word, use IInteractable().)
    /// </summary>
    public interface IUsable
    {
        /// <summary>
        /// To use the item, called when using a selected item.
        /// </summary>
        public void OnUse();

        /// <summary>
        /// To use an item from an inventory; for consumables and similar items this should be a 
        /// synonym for OnUse(), probably as a wrapper.  For some items, such as weapons and tools,  
        /// that must be eqipt to use this would instead equipt the item to the its default slot.
        /// </summary>
        public void OnActivate();

        

    }


}