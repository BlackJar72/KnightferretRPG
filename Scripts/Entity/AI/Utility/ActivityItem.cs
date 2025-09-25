using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityItem : MonoBehaviour, IActivityObject
    {

        [SerializeField] ENeed theNeed;
        [Range(0.0f, 1.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;

        private float desireability;

        public ENeed TheNeed => theNeed;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;



        public float GetUtility(ITalkerAI entity)
        {
            desireability = desireabilityFactor + Mathf.Sqrt(desireabilityFactor / (timeToDo + 60f) + 1) - 1;
            desireability *= entity.GetNeed(theNeed).GetDrive();
            desireability /= 2.0f; 
            return desireability;
        }





    }



}