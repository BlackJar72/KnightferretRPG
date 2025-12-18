using UnityEngine;


namespace kfutils.rpg {


    public class WorldEffect : MonoBehaviour, IHaveStringID
    {
        public sealed class Data
        {
            public readonly string typeID;
            public readonly string id;
            public readonly double timeToDie;
            public readonly TransformData trans;
            public Data(string typeID, string id, double timeToDie, Transform transform)
            {
                this.typeID = typeID;
                this.id = id;
                this.timeToDie = timeToDie;
                this.trans = new(transform);
            }
        }


        [SerializeField] protected string typeID; 

        protected string id;

        protected ChunkManager chunk;

        public string ID => id;
        public string TypeID => typeID;


        public virtual void Create()
        {
            id = System.Guid.NewGuid().ToString();
            // TODO: Register
        }


        public virtual void ReInit()
        {
            
        }


        protected void SendToCorrectChunk() {
            ChunkManager newchunk = WorldManagement.GetChunkFromTransform(transform);
            if(newchunk != null) transform.SetParent(newchunk.LooseItems);
            if(chunk != newchunk) {
                if(chunk != null) chunk.Data.EffectsInChunkList.Remove(id);
                chunk = newchunk;
                chunk.Data.AddItem(id);
            }
            ItemData data = ItemManagement.GetItem(id);
            if(data != null) data.transformData = transform.GetGlobalData();
        }


        public virtual void StoreData()
        {
            Data data = new(typeID, id, double.PositiveInfinity, transform);
        }


        public virtual void SetData(Data data)
        {
            typeID = data.typeID;
            id = data.id;
        }
        

    }

}
