using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public enum AIStateID
    {
        idle = 0,
        wander = 1,
        work = 2,
        aggro = 3,
        flee = 4,
        death = 5,
        special = 6,
        talk = 7
    }


    /*
    There are two ways I could do this:

        (1) I could keep these are pure C# objects, contructing them when the character is created (probable in Awake) 
        as was the original plan.  This allows maximum control and avoids any overhead or possible gotchas associated 
        with scriptable objects.  The one drawback is each entity type (possibly including unique individuals) would 
        need to be coded as a separate subclass of EntityActor or EntityTalking. 

        (2) I could make this SystemSerializable and make AIState a scriptable object (which would act as a prototype). 
        As I now realize scriptable objects can be cloned with Instantiate into separate run-time versions I could 
        replace all the provided scriptable objects with newly instatiated clones (again, probably in Awake).  The 
        cloned versions could contain instance specific data.  This would allow for a more fully component based approach 
        and creating specific entities in the inspector, at least to a point.  One drawback is that the owner of the 
        AI could not be a readonly field set in the constructor.

    */


    [System.Serializable]
    public class AIStates
    {
        [SerializeField] AIState idle;
        [SerializeField] AIState wander;
        [SerializeField] AIState work;
        [SerializeField] AIState talk;
        [SerializeField] AIState aggro;
        [SerializeField] AIState flee;
        [SerializeField] AIState death;

        [SerializeField] AIState[] special;


        AIState current;
        AIState previous;
        AIStateID currentID;


        public void Init(EntityActing owner)
        {
            idle = Object.Instantiate(idle);
            idle.Init(owner);
            wander = Object.Instantiate(wander);
            wander.Init(owner);
            work = Object.Instantiate(work);
            work.Init(owner);
            talk = Object.Instantiate(talk);
            talk.Init(owner);
            aggro = Object.Instantiate(aggro);
            aggro.Init(owner);
            flee = Object.Instantiate(flee);
            flee.Init(owner);
            death = Object.Instantiate(death);
            death.Init(owner);
            SetState(owner.DefaultState);
        }


        public void Act()
        {
            current.Act();
        }


        public void SetState(AIStateID state)
        {
            previous = current == null ? idle : current;
            currentID = state;
            switch (state)
            {
                case AIStateID.idle:
                    ReallySetState(idle);
                    break;
                case AIStateID.wander:
                    ReallySetState(wander);
                    break;
                case AIStateID.work:
                    ReallySetState(work);
                    break;
                case AIStateID.talk:
                    ReallySetState(talk);
                    break;
                case AIStateID.aggro:
                    ReallySetState(aggro);
                    break;
                case AIStateID.flee:
                    ReallySetState(flee);
                    break;
                case AIStateID.death:
                    ReallySetState(death);
                    break;
                default:
                    break;
            }
        }


        private void ReallySetState(AIState next)
        {
            if (current == null)
            {
                previous = next;
            }
            else
            {
                current.StateExit();
                previous = current;
            }
            current = next;
            current.StateEnter();
        }


        public AIStateID GetAIState => currentID;


        // Need to determing if stealth attacks are really stealth
        public bool IsAggro => current == aggro;



    }
    

}
