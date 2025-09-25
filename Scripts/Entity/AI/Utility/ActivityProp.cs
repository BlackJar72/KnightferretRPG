using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityProp : MonoBehaviour, IActivityObject
    {

        public ENeed need;
        public float satisfaction;
        public float timeToDo;
        public Transform actorLocation;

        public bool available = true;
        public bool shareable = false;


        public float GetUtility(EntityMoving entity)
        {
            // PROBLEM: Need reference to need, and these will likely be attached to 
            // EntityTalking or an ITalker/ITalkerAI.  Then, this kind of AI will likely 
            // only be used for intelligent creatures, so EntityLiving may be the wrong 
            // choice here.  Perhaps change the interface to take an ITalkerAI (which also 
            // needs to be created.)
            throw new System.NotImplementedException();
        }





    }



}