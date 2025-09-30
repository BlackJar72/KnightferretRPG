using UnityEngine;


namespace kfutils.rpg
{

    public interface ITalkerAI : ITalker, ICombatantAI
    {

        public Need GetNeed(ENeed need);
        public ActivityChooser NeedChooser { get; }
        public ActivityHolder ChooseNeedActivity(); 




    }



}
