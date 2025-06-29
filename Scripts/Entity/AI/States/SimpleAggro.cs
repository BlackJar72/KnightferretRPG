using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/SimpleAggro", fileName = "Simple Aggro", order = 20)]
    public class SimpleAggro : AIState
    {
        [SerializeField] float attackTime = 1.0f;

        private float destUpdateTime;
        private float nextAttackTime;


        public override void StateEnter()
        {
            destUpdateTime = Time.time + Random.value;
        }


        public override void StateExit() {/*Do Nothing*/}


        public override void Act()
        {
            if (Time.time > destUpdateTime)
            {
                owner.SetDestination(EntityManagement.playerCharacter.transform.position);
                destUpdateTime += 0.25f;
            }
        }
        

    }


}