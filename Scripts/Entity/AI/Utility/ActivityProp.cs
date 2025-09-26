using System.Runtime.CompilerServices;
using UnityEngine;


namespace kfutils.rpg
{

    /*
    There are relatively few world objects ("props") that would make sense in the setting of 
    the actual planned game that would just supply a need directly -- less so than in a 
    modern setting where appliances like TVs exist.  It seems like most need suppliers should 
    actually be item suppliers that advertise the values of the items they supply, thus allowing 
    those items to be used with the next behavior choice.  This would work better with action 
    planning implemented where, for example, hunger might trigger eat -> get food.  However, 
    placing the item in the inventory ought to be a start to be select on the next AI call 
    should work.  However, a system for thise still needs to be set up.
    */


    /// <summary>
    /// Reperesnts items in the world that you may want NPC to interact with. The 
    /// word "prop" in the name is used to avoid confusion and because "object" is 
    /// used elsewhere. 
    /// </summary>
    public class ActivityProp : MonoBehaviour, IActivityObject
    {

        [SerializeField] ENeed theNeed;
        [Range(0.0f, 1.0f)][SerializeField] float desireabilityFactor = 1.0f;
        [SerializeField] float timeToDo;
        [SerializeField] Transform actorLocation;
        [SerializeField] bool shareable = false;

        public bool available = true;

        private float desireability;

        public ENeed TheNeed => theNeed;
        public float DesireabilityFactor => desireabilityFactor;
        public float TimeToDo => timeToDo;
        public Transform ActorLocation => actorLocation;
        public bool Shareable => shareable;



        public virtual float GetUtility(ITalkerAI entity)
        {
            if (available || shareable)
            {
                desireability = desireabilityFactor + Mathf.Sqrt(desireabilityFactor / (timeToDo + 60f) + 1) - 1;
                desireability *= entity.GetNeed(theNeed).GetDrive();
                desireability /= Mathf.Sqrt((entity.GetTransform.position - actorLocation.position).magnitude) + 3;
                return desireability;
            }
            else
            {
                return 0.0f;
            }
        }


        public ActivityHolder GetActivityOption(ITalkerAI entity)
        {
            return new ActivityHolder(this, GetUtility(entity));
        }
    }



}
