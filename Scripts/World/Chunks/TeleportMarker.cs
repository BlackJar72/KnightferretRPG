using UnityEngine;

namespace kfutils.rpg {

    public class TeleportMarker : MonoBehaviour {

        [SerializeField] string id;

        public string ID => id;


        void OnEnable() {
            WorldManagement.teleportMarkers.Add(id, this);
        }


        void OnDisable() {
            WorldManagement.teleportMarkers.Remove(id);
        }

    }

}
