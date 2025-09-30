using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    public class EntityTalking : EntityActing, ITalkerAI
    {

        [SerializeField] Personality personality;
        [SerializeField] CoreNeeds needs;
        [SerializeField] ActivityChooser needChooser;

        public ActivityChooser NeedChooser => needChooser;


        void Awake()
        {
            base.Awake();
            needs.Init(this);
        }


        public Need GetNeed(ENeed need)
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
