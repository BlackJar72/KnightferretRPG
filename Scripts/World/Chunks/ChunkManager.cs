using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace kfutils.rpg {

    /// <summary>
    /// A class for managing resources and other things related to game play within a world chunk.
    /// 
    /// This must *NOT* be a singleton, as there should be one for each loaded chunk, handling things 
    /// related specifically to that chunk.  For manangement of the world as a whole, including 
    /// coordination across / between chunks, see WorldManagement.
    /// </summary>
    public class ChunkManager : MonoBehaviour {

        string id = null;

        [SerializeField] Transform looseItems;
        
        public Vector2Int location;

        private ChunkData data;

        private List<IActivityObject> activityProps = new List<IActivityObject>();

        private List<ITalkerAI> activityNPCs = new List<ITalkerAI>();


        public ChunkData Data => data;
        public Transform LooseItems => looseItems;
        public List<IActivityObject> ActivityProps => activityProps;
        public List<ITalkerAI> ActivityNPCs => activityNPCs;


        public void SetID(string id)
        {
            this.id = id;
        }


        public override string ToString()
        {
            return id + " at " + location;
        }


        public void AddActivityProp(IActivityObject prop)
        {
            if(!activityProps.Contains(prop)) activityProps.Add(prop);
        }


        public void AddActivityNPC(ITalkerAI npc)
        {
            if(!activityNPCs.Contains(npc)) activityNPCs.Add(npc);
        }


        public void Init()
        {
            data = WorldManagement.GetChunkData(id);
            if (data == null)
            {
                data = new ChunkData(id);
                if (data.Clean) FirstInit();
                else LaterInit(); // Could be "dirty" if loaded from a save (TODO)
                WorldManagement.StoreChunkData(data);
            }
            else
            {
                LaterInit();
            }
        }


        private void FirstInit() {
            data.MakeDirty(); // Only treat as new once
            ItemPlaceholder[] items = transform.parent.GetComponentsInChildren<ItemPlaceholder>();
            for(int i = 0; i < items.Length; i++) {
                ItemManagement.AddItem(items[i].GetData());
                data.AddItem(items[i].ID);
                Destroy(items[i].gameObject);                
            }
            for (int i = 0; i < data.ItemsInChunkList.Count; i++)
            {
                ItemData itemData = ItemManagement.GetItem(data.ItemsInChunkList[i]);
                ItemInWorld spawned = Instantiate(itemData.Prototype.InWorld, looseItems);
                spawned.transform.SetDataGlobal(itemData.TransformData);
                spawned.SetID(itemData.ID);
                spawned.chunk = this;
                if (items[i].StartWithPhysics) spawned.EnablePhysics();
            }
            // Should not need to deal with effects here, as almost by definition these are added later;
            // if an effect is needed in advance, a paceholder could simply spawn it, as it will register
            // itself when spawned.
        }


        private void LaterInit() {
            ItemPlaceholder[] items = transform.parent.GetComponentsInChildren<ItemPlaceholder>();
            for(int i = 0; i < items.Length; i++) {
                Destroy(items[i].gameObject);                
            }
            for(int i = 0; i < data.ItemsInChunkList.Count; i++) {
                ItemData itemData = ItemManagement.GetItem(data.ItemsInChunkList[i]);
                ItemInWorld spawned = Instantiate(itemData.Prototype.InWorld, looseItems);
                spawned.transform.SetDataGlobal(itemData.TransformData);
                spawned.SetID(itemData.ID);
                spawned.chunk = this;
                if(itemData.physics) spawned.EnablePhysics(); 
            }
            for(int i = 0; i < data.EffectsInChunkList.Count; i++) {
                WorldEffect.Data effectData = ObjectManagement.GetEffect(data.EffectsInChunkList[i]);
                Debug.Log("");
                Debug.Log("data.EffectsInChunkList[i] " + data.EffectsInChunkList[i]);
                Debug.Log("effectData " + effectData);
                Debug.Log("itemData.Prototype " + effectData.TypeID);
                Debug.Log("ObjectManagement.GetPrototype(effectData.typeID) " + ObjectManagement.GetPrototype(effectData.TypeID));
                Debug.Log("ObjectManagement.GetPrototype(effectData.typeID).Effect " + ObjectManagement.GetPrototype(effectData.TypeID).Effect);
                WorldEffect effect = ObjectManagement.GetPrototype(effectData.TypeID).Effect;
                WorldEffect spawned = Instantiate(effect, looseItems);
                spawned.transform.SetDataGlobal(effectData.TransData);
                spawned.SetData(effectData);
                spawned.SetChunkDirect(this);
            }
        }





    }

}