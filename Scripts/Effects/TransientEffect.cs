using UnityEngine;


namespace kfutils.rpg {


    public class TransientEffect : WorldEffect
    {
        [SerializeField] float duration;

        private double timeToDie;

        public override double TimeToDie => timeToDie;
        public override bool ShouldDie => timeToDie < WorldTime.time;


        private void Update()
        {
            if(WorldTime.time > timeToDie)
            {
               EndEffect();
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
            ObjectManagement.AddEffect(data);
        }


        public override void UpdateData()
        {
            Data data = new(typeID, id, timeToDie, transform);
            ObjectManagement.UpdateEffect(data);
        }


        public override void SetData(Data data)
        {
            typeID = data.TypeID;
            id = data.ID;
            timeToDie = data.TimeToDie;
        }

    }

}
