using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityItem : MonoBehaviour, IActivityObject
    {

        [SerializeField] ENeed theNeed;
        [Range(0.0f, 1.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] AbstractAction useAction;

        public ENeed TheNeed => theNeed;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;

        public AbstractAction UseAction => useAction;


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }


        public virtual float GetUtility(ITalkerAI entity)
        {
            float desireability = desireabilityFactor + Mathf.Sqrt(desireabilityFactor / (timeToDo + 60f) + 1) - 1;
            desireability *= entity.GetNeed(theNeed).GetDrive();
            desireability /= 2.0f;
            return desireability;
        }





    }



}
