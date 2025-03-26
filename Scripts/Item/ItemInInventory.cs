using System;
using UnityEngine;


namespace kfutils.rpg {
    
    [Serializable]
    public class ItemInInventory
    {
        public Sprite icon;
        [SerializeField] bool isStackable;
        
        public bool IsStackable {get => isStackable; }


    }


}
