using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{


    [System.Serializable]
    public class InventorySaveData
    {
        public string id;
        public string ID => id;
        public List<string> itemIDs;
        public float weight;
    }


    [System.Serializable]
    public class EquiptSaveData
    {
        public string id;
        public string ID => id;
        public string[] itemIDs;
        public bool belongsToPC;
        public float weight;
    }


    [System.Serializable]
    public class CharacterInventorySaveData : InventorySaveData
    {
        public EquiptSaveData equiptSlots;
        public string ownerID;
        public string OwnerID => ownerID;
        public Money money;
        public bool isPlayerInventory;
    }


}
