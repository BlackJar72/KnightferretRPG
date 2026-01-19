using UnityEngine;


namespace kfutils.rpg
{

    public abstract class AISubState : AIState
    {
        protected AIState parent;


#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [System.Obsolete]
        public sealed override void Init(EntityActing character)
        {
            Debug.LogError("Trying to initialize AISubState without parent; use Init(EntityActing, AIState) instead.");
            throw new System.Exception("Trying to initialize AISubState without parent; use Init(EntityActing, AIState) instead.");
        }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member


        public virtual void Init(EntityActing character, AIState parent)
        {
            if (owner == null)
            {
                owner = character;
                this.parent = parent;
            }
            else
            {
                Debug.LogError("Trying to call init more that once ona same AISubState!");
                throw new System.Exception("Trying to call init more that once ona same AISubState!");
            }        
        }


    }

}