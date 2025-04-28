using UnityEngine;

namespace kfutils.rpg {

    public class Worldspace : MonoBehaviour {

        [SerializeField] float seaLevel;


        public float SeaLevel => seaLevel;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            WorldManagement.SetWorldspace(this);
        }
    }

}
