using UnityEngine;
using UnityEngine.UI;
using kfutils.rpg.ui;

namespace kfutils.rpg {

    public interface IInvReferant
    {
        public delegate void ReferantRemoval(IInvReferant referant);

        /// <summary>
        /// This must be implement in such a way as to remove the 
        /// referant.  This should be invoked when the referant 
        /// either should cease to exist (e.g., item dropped or 
        /// destroyed) or is being mover to a location that would 
        /// make the reference invalide (e.g., moving an item on a 
        /// hot bar from the player inventory into a separate 
        /// container).
        /// </summary>
        public event ReferantRemoval SignalReferantRemoval;

        public Sprite Icon { get; }
        public int StackSize { get; }
        public int Slot { get; }

        public void AddReference(IInvReference reference);
        public void RemoveReference(IInvReference reference);
        public void RemoveReferant();


    }


}
