using UnityEngine;


namespace kfutils.rpg
{

    public class ActivitySocial : MonoBehaviour, IActivityObject
    {


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
