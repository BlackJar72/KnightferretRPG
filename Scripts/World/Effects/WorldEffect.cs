using Unity.Mathematics;
using UnityEngine;


namespace kfutils.rpg {


    public class WorldEffect : MonoBehaviour, IHaveStringID
    {
        [System.Serializable]
        public class Data
        {
            [SerializeField] string typeID;
            [SerializeField] string id;
            [SerializeField] double timeToDie;
            [SerializeField] TransformData transform;
        
            public string TypeID => typeID;
            public string ID => id;
            public double TimeToDie => timeToDie;
            public TransformData TransData => transform;

            public Data(string typeID, string id, double timeToDie, Transform transform)
            {
                this.typeID = typeID;
                this.id = id;
                this.timeToDie = timeToDie;
                this.transform = new(transform);
            }
        }


        [SerializeField] protected string typeID; 
        [SerializeField] bool alwaysUpright = true;

        protected string id;

        protected ChunkManager chunk;

        public string ID => id;
        public string TypeID => typeID;
        public virtual double TimeToDie => double.PositiveInfinity;
        public virtual bool ShouldDie => false;

        public virtual void Create()
        {
            id = System.Guid.NewGuid().ToString(); 
            if(alwaysUpright) transform.eulerAngles = Vector3.zero;
            StoreData();           
            SendToCorrectChunk();
        }


        protected void SendToCorrectChunk() {
            ChunkManager newchunk = WorldManagement.GetChunkFromTransform(transform);
            if(newchunk != null) transform.SetParent(newchunk.LooseItems);
            if(chunk != newchunk) {
                if(chunk != null) chunk.Data.EffectsInChunkList.Remove(id);
                chunk = newchunk;
                chunk.Data.AddEffect(id);
            }
            Data data = ObjectManagement.GetEffect(id);
            if(data != null) data.TransData.SetDataFrom(transform);
            else StoreData();
        }


        public virtual void StoreData()
        {
            Data data = new(typeID, id, double.PositiveInfinity, transform);
            ObjectManagement.AddEffect(data);
        }


        public virtual void SetData(Data data)
        {
            typeID = data.TypeID;
            id = data.ID;
        }


        public void SetChunkDirect(ChunkManager chunkManager)
        {
            chunk = chunkManager;
        }


        public void EndEffect()
        {
            chunk.Data.EffectsInChunkList.Remove(id);
            ObjectManagement.RemoveEffect(id);
            Destroy(gameObject);
        }


        public override string ToString()
        {
            return typeID + "[" + id + "]";
        }
        

    }

}
