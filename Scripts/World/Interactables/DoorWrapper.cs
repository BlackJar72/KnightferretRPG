#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace kfutils.rpg
{

    public class DoorWrapper : MonoBehaviour, IInteractable, IAutoAssignID 
    {
        [SerializeField] SimpleOpener door;
        [SerializeField] string id;
        [SerializeField] bool defaultOpenState;

        private bool state = false; // True if changed from default
        private bool dirty = false; // has state changed

        public string ID => id;


        public void BeAssignedID()
        {
            #if UNITY_EDITOR
            id = "DOOR_" + System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
            #endif
        }


        public void Use(GameObject other) {
            state = !state;
            door.Activate();
            dirty = true; 
        }


        void OnEnable()
        {
            state = ObjectManagement.GetTimedData(id);
            if(state) door.SetStateOpen(!defaultOpenState);
            else door.SetStateOpen(defaultOpenState);
            dirty = false; 
        }


        void OnDisable()
        {
            if(state || dirty) ObjectManagement.SetTimedData(id, new(state ,WorldTime.time + (WorldTime.DAY / GameConstants.TIME_SCALE / 3.0)));            
        }


    }


}
