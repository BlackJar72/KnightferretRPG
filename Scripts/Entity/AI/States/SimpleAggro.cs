using Unity.Transforms;
using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/Simple Aggro", fileName = "SimpleAggro", order = 20)]
    public class SimpleAggro : AIState, INotifiableT<MeleeTrigger>
    {
        [SerializeField] float attackTime = 1.0f;

        private float destUpdateTime;
        private float nextAttackTime;
        private bool shouldMelee = false;
        private float pauseTime;


        public override void StateEnter()
        {
            owner.SetMoveType(MoveType.run);
            owner.meleeTrigger.Notifiable = this;
            owner.meleeTrigger.enabled = true;
            owner.meleeTrigger.gameObject.SetActive(true);
            destUpdateTime = Time.time + Random.value;
            owner.StartMoving();
        }


        public override void StateExit()
        {
            if(owner.Alive) owner.SetMoveType(MoveType.walk);
            owner.meleeTrigger.enabled = false;            
            owner.meleeTrigger.gameObject.SetActive(false);
        }


        public override void Act()
        {
            if (shouldMelee)
            {
                owner.StopMoving();
                owner.SetMoveType(MoveType.idle);
                if (Time.time > nextAttackTime) MeleeAttack();
            }
            else
            {
                if (Time.time > destUpdateTime)
                {
                    owner.SetMoveType(MoveType.run);
                    owner.SetDestination(EntityManagement.playerCharacter.transform.position);
                    destUpdateTime += 0.1f;
                }
            }
            // FIXME: If used in a real game, checking for other enemies and switching target 
            //        if such enemies are present.
            if (!owner.targetEnemy.Alive) owner.BasicStates.SetState(owner.DefaultState);
        }


        private void MeleeAttack()
        {
            float delayFactor = 1.0f + (Random.value * 0.5f);
            nextAttackTime = Time.time + (attackTime * delayFactor) + (Random.value * 0.25f) + 0.25f;// - (Random.value * 0.25f);
            // TODO: Attack 
            owner.MeleeAttack(); 
        }


        public void BeNotified(MeleeTrigger notifier)
        {
            shouldMelee = notifier.Triggered;
        }


        public override void Pause()
        {
            pauseTime = Time.time; 
        }


        public override void Resume()
        {
            destUpdateTime = Time.time;
            pauseTime = destUpdateTime - pauseTime;
            attackTime += pauseTime;
        }



    }


}