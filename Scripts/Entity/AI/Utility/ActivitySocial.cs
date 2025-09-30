using UnityEngine;


namespace kfutils.rpg
{

    public class ActivitySocial : MonoBehaviour, IActivityObject
    {

        public AbstractAction UseAction => throw new System.NotImplementedException();


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            throw new System.NotImplementedException();
        }





    }



}
