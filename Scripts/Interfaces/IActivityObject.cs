using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// The object of an activity, where the character doing the activity is the subject. 
    /// This could be an (usually inteactable) object in the world, and inventory item, 
    /// a spell or similar ability, another character, or even nothing (effective, the 
    /// subject is the object).  Thus, it needs to be an interface as different objects 
    /// types may need to be handled differently. 
    /// </summary>
    public interface IActivityObject
    {
        public float GetUtility(ITalkerAI entity); // This should probably table the subject as a parameter(?) 
        public ActivityHolder GetActivityOption(ITalkerAI entity);
        public AbstractAction UseAction { get; }
        public float Satisfaction { get; }
        public float TimeToDo { get; }
        public ENeed GetNeed { get; }
        public EObjectActivity ActivityType { get; }



    }

}
