using UnityEngine;

namespace kfutils.rpg.ui {

    public interface IInvReference
    {
        IInvReferant Item { get; }
        
        public void OnReferantRemoved(IInvReferant referant);

        public void SetReferant(IInvReferant referant);
        public void ClearReferant();


    }


}
