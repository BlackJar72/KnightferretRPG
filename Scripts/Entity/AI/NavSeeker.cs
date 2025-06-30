using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class NavSeeker : MonoBehaviour
    {
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
            agent.isStopped = stopped || (separation.sqrMagnitude > 1.0f);
        }
        
    }


}