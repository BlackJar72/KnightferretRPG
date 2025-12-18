using UnityEngine;


namespace kfutils.rpg {


    public class TransientEffect : WorldEffect
    {
        [SerializeField] float duration;

        private double timeToDie;


        private void Update()
        {
            if(WorldTime.time > timeToDie)
            {
                // TODO: De-register
                Destroy(gameObject);
            }
        }


        public override void Create()
        {
            timeToDie = WorldTime.time + duration;
            base.Create();
        }


        public override void StoreData()
        {
            Data data = new(typeID, id, timeToDie, transform);
        }


        public override void SetData(Data data)
        {
            typeID = data.typeID;
            id = data.id;
            timeToDie = data.timeToDie;
        }

    }

}
