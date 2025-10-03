using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    public class EntityTalking : EntityActing, ITalkerAI
    {

        [SerializeField] Personality personality;
        [SerializeField] CoreNeeds needs;
        [SerializeField] ActivityChooser needChooser;
        [SerializeField] SelfActivity[] selfActivities;

        public ActivityChooser NeedChooser => needChooser;
        public CoreNeeds GetNeeds => needs;
        public SelfActivity[] SelfActivities => selfActivities;


        protected override void Awake()
        {
            base.Awake();
            needs.Init(this);
        }


        protected override void StoreData()
        {
            base.StoreData();
            // TODO: Store Data
        }


        protected override void LoadData()
        {
            base.LoadData();
            // TODO: Store Data
        }


        protected override void Update()
        {
            base.Update();
            needs.UpdateNeeds();
        }


        public Need GetNeed(ENeedID need)
        {
            return needs.GetNeed(need);
        }


        public ActivityHolder ChooseNeedActivity()
        {
            ChunkManager chunk = GetChunkManager;
            List<IActivityObject> props = chunk.ActivityProps;
            List<ActivityHolder> itemsDiscrete = inventory.GetActivities(EObjectActivity.NEED_DISCRETE, this);
            List<ActivityHolder> itemsConinuous = inventory.GetActivities(EObjectActivity.NEED_CONTINUOUS, this);
            needChooser.PopulateActivityList(this, props);
            needChooser.AddToList(itemsDiscrete, itemsConinuous);
            return needChooser.Choose();
        }





    }


}
