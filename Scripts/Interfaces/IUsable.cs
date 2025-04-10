using Animancer;
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


        public void PlayeUseAnimation(AnimancerLayer animancer, AnimancerState animState);

        

    }


}