using Animancer;
using Unity.VisualScripting;
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

        public AbstractAction UseAnimation { get; }

        /// <summary>
        /// To use the item, called when using a selected item.
        /// </summary>
        public void OnUse(IActor actor);

        public void OnEquipt(IActor actor);

        public void OnUnequipt();

        public void PlayUseAnimation(AnimancerLayer animancer, AnimancerState animState);

        public void PlayEquipAnimation(AnimancerLayer animancer, AnimancerState animState);

        [Tooltip("Stamina cost to use item; usually 0 except for weapons which cost stamina to attack")]
        int StaminaCost { get; } // Stamina cost to use item; usually 0 for non-weapons

        

    }


}