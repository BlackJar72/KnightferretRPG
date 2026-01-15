using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils {

    public class HingeOpener : SimpleOpener {
        public enum Axes {
            X,
            Y,
            Z
        }
        public enum Side {
            left,
            right
        }

        [SerializeField] Transform hinge;
        [SerializeField] Axes axisOfRotation = Axes.Y;
        [SerializeField] Side side = Side.left;
        [SerializeField] float closedAngle = 0f;
        [SerializeField] float openAngle = -85f;
        [SerializeField] float timeToOpen = 1f;
        [SerializeField] bool open;

        private bool moving;
        private Quaternion closedQ;
        private Quaternion openQ;
        private float t, startT;


        // Start is called before the first frame update
        void Start() {
            Vector3 closedEuler = Vector3.zero;
            Vector3 openEuler = Vector3.back;
            if(side == Side.right) {
                openAngle = openAngle - ((openAngle - closedAngle) * 2);
            }
            switch (axisOfRotation) {
                case Axes.X:
                    closedEuler = new Vector3(closedAngle, 0, 0);
                    openEuler = new Vector3(openAngle, 0, 0);
                    break;
                case Axes.Y:
                    closedEuler = new Vector3(0, closedAngle, 0);
                    openEuler = new Vector3(0, openAngle, 0);
                    break;
                case Axes.Z:
                    closedEuler = new Vector3(0, 0, closedAngle);
                    openEuler = new Vector3(0, 0, openAngle);
                    break;
            }
            closedQ = Quaternion.Euler(closedEuler);
            openQ = Quaternion.Euler(openEuler);
            if(open) {
                hinge.transform.localRotation = openQ;
            } else {
                hinge.transform.localRotation = closedQ;
            }
        }


        public override void Open() {
            if(!(moving || open)) DoOpen();
        }


        public override void Close() {
            if(!moving && open) DoClose();
        }


        protected void DoOpen() {
            open = true;
            moving = true;
            startT = Time.time;
            StartCoroutine(Opening());
        }


        protected void DoClose() {
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
                hinge.transform.localRotation = Quaternion.Slerp(closedQ, openQ, t);
                moving = (t < 1f);
            }
        }


        private IEnumerator Closing() {
            while (moving) {
                yield return new WaitForFixedUpdate();
                t = Mathf.Clamp((Time.fixedTime - startT) / timeToOpen, 0f, 1f);
                hinge.transform.localRotation = Quaternion.Slerp(openQ, closedQ, t);
                moving = (t < 1f);
            }
        }
    }

}
