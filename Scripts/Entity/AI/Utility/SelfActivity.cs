using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// Used to represent activities done by a character without use of any particualar 
    /// object, item, etc., but only involving themself.  E.g., idling or wandering.
    /// </summary>
    public class SelfActivity : IActivityObject
    {
        [SerializeField] AbstractAction useAction;
        [SerializeField] float timeToDo;

        public AbstractAction UseAction => useAction;
        public float Satisfaction => 0.0f;
        public float TimeToDo => timeToDo;
        public ENeeds GetNeed => ENeeds.NONE;
        public EObjectActivity ActivityType => throw new System.NotImplementedException();


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public float GetUtility(ITalkerAI entity)
        {
            return (Random.value * 0.5f) + 0.1f; // This will likely need to be tweaked
        }
    }


}
