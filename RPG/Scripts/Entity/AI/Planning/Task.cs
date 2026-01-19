using UnityEngine;


namespace kfutils.rpg
{

    public class Task
    {

        [Tooltip("The primary action component, representing an action and object (which may or may not be an actual item/object)")]
        public TaskComponent verbObject;
        [Tooltip("An optional task component representing an object acted on")]
        public TaskComponent prepObject;

    }


    public class TaskComponent
    {
        
    }



}