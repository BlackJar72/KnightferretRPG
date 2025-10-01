using UnityEngine;


namespace kfutils.rpg
{

    public interface ITalkerAI : ITalker, ICombatantAI
    {

        public Need GetNeed(ENeedID need);

        public CoreNeeds GetNeeds { get; }
        public ActivityChooser NeedChooser { get; }
        public ActivityHolder ChooseNeedActivity(); 




    }



}
