using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class NavSeeker : MonoBehaviour
    {
        public const float MAX_DIST = 1.0f;
        public const float MAX_DIST_SQR = MAX_DIST * MAX_DIST; 

        [SerializeField] EntityMoving parent;
        private NavMeshAgent agent;
        public bool stopped = true;


        public NavMeshAgent Agent => agent;


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            transform.parent = transform.parent.parent;
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 separation = transform.position - parent.transform.position;
            agent.isStopped = stopped || (separation.sqrMagnitude > MAX_DIST_SQR);
        }
        
    }


}