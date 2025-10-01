using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityItem : MonoBehaviour, IActivityObject
    {

        // QUESTION: Shouod this exist, or be combined with ItemPrototype???

        [SerializeField] ENeed theNeed;
        [SerializeField] EObjectActivity activityType;
        [Range(0.0f, 1.0f)][SerializeField] float satisfaction;
        [Range(0.0f, 2.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] AbstractAction useAction;

        public ENeed TheNeed => theNeed;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;

        public AbstractAction UseAction => useAction;

        public ENeed GetNeed => theNeed;
        public float Satisfaction => satisfaction;

        public EObjectActivity ActivityType => activityType;

        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public virtual float GetUtility(ITalkerAI entity)
        {
            float desireability = (satisfaction * desireabilityFactor)
                    + Mathf.Sqrt((satisfaction * desireabilityFactor) / (timeToDo + 60f) + 1) - 1;
            desireability *= entity.GetNeed(theNeed).GetDrive();
            desireability /= 2.0f;
            return desireability;
        }





    }



}
