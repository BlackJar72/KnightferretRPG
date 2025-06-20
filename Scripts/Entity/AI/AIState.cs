using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// An AI state.  These could contain simple code directly controlling actions, 
    /// allowing for a basic FSM AI (similar to Doom or Halflife) or contain context
    /// specific utility AIs or behavior trees -- or a hybrid of both, with simple 
    /// code for simpler or transient states.
    /// </summary>
    public abstract class AIState
    {
        protected readonly EntityActing owner;


        public EntityActing Owner => owner;


        public AIState(EntityActing actor)
        {
            owner = actor;
        }

    }

}
