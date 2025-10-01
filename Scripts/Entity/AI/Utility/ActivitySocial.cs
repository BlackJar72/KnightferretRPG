using UnityEngine;


namespace kfutils.rpg
{

    // QUESTION: Should this be its own monobehavior, or should ITalker or ITalkerAI inherit IActivity Object???

    public class ActivitySocial : MonoBehaviour, IActivityObject
    {

        public AbstractAction UseAction => throw new System.NotImplementedException();
        public float TimeToDo => throw new System.NotImplementedException();

        public ENeed GetNeed => throw new System.NotImplementedException();

        public EObjectActivity ActivityType => throw new System.NotImplementedException();

        public float Satisfaction => throw new System.NotImplementedException();

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
