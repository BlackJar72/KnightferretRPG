using UnityEngine;


namespace kfutils.rpg {

    public class EntityTalking : EntityActing, ITalkerAI
    {
        [SerializeField] ActivityChooser needChooser;

        public ActivityChooser NeedChooser => needChooser;
        


        public Need GetNeed(ENeed need)
        {
            throw new System.NotImplementedException();
        }

        public IActivityObject ChooseNeedActivity()
        {
            throw new System.NotImplementedException();
        }


        
    }


}
