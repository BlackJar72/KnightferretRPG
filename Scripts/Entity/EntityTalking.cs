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
            List<ActivityProp> props = chunk.ActivityProps;
            needChooser.PopulateActivityList(this, props);
            return needChooser.Choose();
        }





    }


}
