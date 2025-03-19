using kfutils.rpg;
using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg {


    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMoving : EntityLiving
    {

        public NavMeshAgent navMeshAgent;

         // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start() {
            base.Start();
            navMeshAgent = GetComponent<NavMeshAgent>();            
        }
        

        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/



    }


}
