using UnityEngine;
using UnityEngine.AI;


namespace kfutils.rpg {


    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMoving : EntityLiving {

        // This is to make sure this is never overriden into something harmful.
        protected sealed override void MakePC(string id) { base.MakePC(ID); }

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
