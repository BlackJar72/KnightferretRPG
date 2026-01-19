using UnityEngine;


namespace kfutils.rpg {


    /// <summary>
    /// Core holder of entity data, with components for each level of entity complexity.
    /// 
    /// New idea -- I will NOT be completely abstracting the data away from the MonoBehaviours. 
    /// Instead, on load they will check if they are registered in with EntityManagement; if so 
    /// they will load (and overwrite) their data from there, if not they will create a copy of 
    /// their data and store it.  This class and those that act as its components will be the 
    /// recepticle for the saved data, through which it is saved, loaded, and kept consistent.
    /// 
    /// This class (and others in this directory, which are its components) must NOT be System.Serializable, 
    /// as that would cause there data to be serialized with the game object holding its MonoBehaviour 
    /// counterpart.  Instead, they must be maintained statically trhought EntityManagement class and 
    /// serialed outside noraml Unity serialization when saving is required.
    /// 
    /// (I'm not completely satisfied with this system, since it increases the amount of data that needs 
    /// to be stored in ram, but its relatively simple and should be fairly stable and safe.)
    /// </summary>
    public class EntityData { 

        private string id;
        // TODO: Times stamp -- need persistent world time first
        
        public LivingData livingData;
        public MovingData movingData;
        public ActingData actingData;
        public TalkingData talkingData;


        public string ID => id; 


        public EntityData(string ID) {
            id = ID;
        }
        



    }

}