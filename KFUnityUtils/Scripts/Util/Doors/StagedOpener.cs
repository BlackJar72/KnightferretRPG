using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;



namespace kfutils {

    public class StagedOpener : SimpleOpener {

        [System.Serializable]
        public struct DoorPos {
            [SerializeField] public Transform transform;
            [SerializeField] public float timeToReach;
        }

        [System.Serializable]
        private struct Stage {
            public Vector3 start, end;
            public float timeToMove;
            public Stage(Vector3 start, Vector3 end, float timeToReach) {
                this.start = start;
                this.end = end;
                this.timeToMove = timeToReach;
            }
        }

        [SerializeField] GameObject door;
        [SerializeField] DoorPos[] positions;
        [SerializeField] bool open;

        [SerializeField] private Stage[] stages;
        private bool moving;
        private float t, startT;


        void Awake() {
            if(positions.Length > 1) {
                stages = new Stage[positions.Length - 1];
                for (int i = 0; i < stages.Length; i++) {
                    stages[i] = new Stage(positions[i].transform.localPosition,
                                          positions[i + 1].transform.localPosition,
                                          positions[i + 1].timeToReach);
                }
            } else {
                stages = null;
            }
        }


        void Start() {
            if(positions.Length > 0) {
                if (open && (positions.Length > 1)) {
                    door.transform.localPosition = stages[stages.Length - 1].end;
                } else {
                    door.transform.localPosition = stages[0].start;
                }
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
            moving = stages != null;
            startT = Time.time;
            StartCoroutine(Opening());
        }


        private void DoClose() {
            open = false;
            moving = stages != null;
            startT = Time.time;
            StartCoroutine(Closing());
        }


        public override void Activate() {
            if (moving) return;
            else if (open) DoClose();
            else DoOpen();
        }


        private IEnumerator Opening() {
            int i = 0;
            while (moving) {
                yield return new WaitForFixedUpdate();
                t = Mathf.Clamp((Time.fixedTime - startT) / stages[i].timeToMove, 0f, 1f);
                door.transform.localPosition = Vector3.Lerp(stages[i].start, stages[i].end, t);
                if(t >= 1f) {
                    startT = Time.fixedTime;
                    i++;
                }
                moving = (i < stages.Length);
            }
        }


        private IEnumerator Closing() {
            int i = stages.Length - 1;
            while (moving) {
                yield return new WaitForFixedUpdate();
                t = Mathf.Clamp((Time.fixedTime - startT) / stages[i].timeToMove, 0f, 1f);
                door.transform.localPosition = Vector3.Lerp(stages[i].end, stages[i].start, t);
                if(t >= 1f) {
                    startT = Time.fixedTime;
                    i--;
                }
                moving = (i > -1);
            }
        }

    }


}