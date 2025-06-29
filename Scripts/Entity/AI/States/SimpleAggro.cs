using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/SimpleAggro", fileName = "Simple Aggro", order = 20)]
    public class SimpleAggro : AIState, INotifiableT<MeleeTrigger>
    {
        [SerializeField] float attackTime = 1.0f;

        private float destUpdateTime;
        private float nextAttackTime;
        private bool shouldMelee = false;


        public override void StateEnter()
        {
            owner.meleeTrigger.Notifiable = this;
            owner.meleeTrigger.enabled = true;
            owner.meleeTrigger.gameObject.SetActive(true);
            destUpdateTime = Time.time + Random.value;
        }


        public override void StateExit()
        {
            owner.meleeTrigger.enabled = false;            
            owner.meleeTrigger.gameObject.SetActive(false);
        }


        public override void Act()
        {
            if (shouldMelee)
            {
                if (Time.time > nextAttackTime) MeleeAttack();
            }
            else if (Time.time > destUpdateTime)
            {
                owner.SetDestination(EntityManagement.playerCharacter.transform.position);
                destUpdateTime += 0.1f;
            }
        }


        private void MeleeAttack()
        {
            nextAttackTime = Time.time + attackTime;
            // TODO: Attack 
            #if UNITY_EDITOR
            Debug.Log(owner.name + " is attacking! ");
            #endif
        }


        public void BeNotified(MeleeTrigger notifier)
        {
            shouldMelee = notifier.Triggered;
        }


    }


}