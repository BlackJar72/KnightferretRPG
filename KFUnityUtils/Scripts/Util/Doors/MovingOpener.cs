using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;



namespace kfutils {

    public class MovingOpener : SimpleOpener {
        [SerializeField] GameObject door;
        [SerializeField] Transform openPos;
        [SerializeField] Transform closedPos;
        [SerializeField] float timeToOpen = 2f;
        [SerializeField] bool open;

        private bool moving;
        private float t, startT;


        void Start() {
            if(open) {
                door.transform.localPosition = openPos.localPosition;
            } else {
                door.transform.localPosition = closedPos.localPosition;
            }
        }


        public override void Open() {
            if(!(moving || open)) DoOpen();
        }


        public override void Close() {
            if(!moving && open) DoClose();
        }


        private void DoOpen() {
            open = true;
            moving = true;
            startT = Time.time;
            StartCoroutine(Opening());
        }


        private void DoClose() {
            open = false;
            moving = true;
            startT = Time.time;
            StartCoroutine(Closing());
        }


        public override void Activate() {
            if (moving) return;
            else if (open) DoClose();
            else DoOpen();
        }


        private IEnumerator Opening() {
            while (moving) {
                yield return new WaitForFixedUpdate();
                t = Mathf.Clamp((Time.fixedTime - startT) / timeToOpen, 0f, 1f);
                door.transform.localPosition = Vector3.Slerp(closedPos.localPosition, openPos.localPosition, t);
                moving = (t < 1f);
            }
        }


        private IEnumerator Closing() {
            while (moving) {
                yield return new WaitForFixedUpdate();
                t = Mathf.Clamp((Time.fixedTime - startT) / timeToOpen, 0f, 1f);
                door.transform.localPosition = Vector3.Slerp(openPos.localPosition, closedPos.localPosition, t);
                moving = (t < 1f);
            }
        }



    }


}
