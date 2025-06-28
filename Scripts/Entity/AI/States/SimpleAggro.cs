using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/SimpleAggro", fileName = "Simple Aggro", order = 20)]
    public class SimpleAggro : AIState
    {

        public override void Act()
        {
            owner.SetDestination(EntityManagement.playerCharacter.transform.position);
        }


    }


}