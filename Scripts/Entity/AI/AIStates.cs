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


        public void Init(EntityActing owner)
        {
            idle = Object.Instantiate(idle);
            idle.Init(owner);
            wander.Init(owner);
            work.Init(owner);
            talk.Init(owner);
            aggro.Init(owner);
            flee.Init(owner);
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
            switch (state)
            {
                case AIStateID.idle:
                    current = idle;
                    break;
                case AIStateID.wander:
                    current = wander;
                    break;
                case AIStateID.work:
                    current = work;
                    break;
                case AIStateID.talk:
                    current = talk;
                    break;
                case AIStateID.aggro:
                    current = aggro;
                    break;
                case AIStateID.flee:
                    current = flee;
                    break;
                case AIStateID.death:
                    current = death;
                    break;
                default:
                    break;
            }
        }
    }

}
