using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public enum AIStateID
    {
        idle,
        wander,
        work,
        aggro,
        chase,
        melee,
        ranged,
        flee,
        death,
        special
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

    I'll have to think about it.
    */


    public class AIStates
    {
        AIState idle;
        AIState wander;
        AIState work;
        AIState aggro;
        AIState chase;
        AIState melee;
        AIState ranged;
        AIState flee;
        AIState death;

        Dictionary<string, AIState> special;
    }

}
