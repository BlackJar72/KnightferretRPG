using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui {
  

   public class Compass : MonoBehaviour {

        [SerializeField] RawImage compassContent;
        [SerializeField] Transform player;
        
        private Rect compassRect;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {}


        // Update is called once per frame
        void Update() {
            float offset = player.eulerAngles.y / 360f;
            compassContent.uvRect = new Rect(offset, 0f, 1f, 1f);            
        }


    }


}
