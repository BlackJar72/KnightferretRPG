using Animancer;
using UnityEngine;


namespace kfutils.rpg
{

    // FIXME??? Do I really need to use these interfaces when a first is a fist and only applies to humanoid using a direct reference?!
    public class Fists : MonoBehaviour, IWeapon, IBlockItem
    {

        [SerializeField] DamageSource damage;
        [SerializeField] AbstractAction doubleAnimation; // Two first (no shield)
        [SerializeField] AbstractAction singleAnimation; // Only right fist (with shield)
        [SerializeField] AbstractAction blockAnimation;
        [SerializeField] ICombatant owner;

        private Collider rightCollider;  // Right first hit collider
        private Collider leftCollider;   // Left first hit collider

        private bool busy = false;
        private bool attacking = false;
        private bool queued = false;
        private int attack = 0;
        private AnimancerState attackState;

        public bool Parriable => false;
        public AbstractAction UseAnimation => singleAnimation;
        public int StaminaCost => 2;
        public int PowerAttackCost => 5;

        public float BlockAmount => 0.25f;
        public float Stability => 0.75f;
        public float ParryWindow => 0.21f;


        /*****************************************************************************************************/
        /*                                       ATTACK METHODS                                              */
        /*****************************************************************************************************/


        public void AttackMelee(ICombatant attacker)
        {
            throw new System.NotImplementedException();
        }


        public void AttackRanged(ICombatant attacker, Vector3 direction)
        {
            Debug.LogError("Trying to perform ranged attack with *FIST*!");
            throw new System.NotImplementedException();
        }


        public void BeBlocked(ICombatant blocker, BlockArea blockArea)
        {
            throw new System.NotImplementedException();
        }


        public float GetAttackSpeed()
        {
            throw new System.NotImplementedException();
        }


        public int GetDamage()
        {
            throw new System.NotImplementedException();
        }


        public void OnEquipt(IActor actor)
        {
            Debug.LogError("Hands are ALWAYS equipped!");
            throw new System.NotImplementedException();
        }


        public void OnUnequipt()
        {
            Debug.LogError("Hands are ALWAYS equipped!");
            throw new System.NotImplementedException();
        }


        public void OnUse(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void OnUseCharged(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void PlayEquipAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        public void PlayUseAnimation(IActor actor)
        {
            throw new System.NotImplementedException();
        }


        /*****************************************************************************************************/
        /*                                       DEFNCE METHODS                                              */
        /*****************************************************************************************************/


        public void BeHit()
        {
            throw new System.NotImplementedException();
        }


        public void BeParried()
        {
            throw new System.NotImplementedException();
        }


        public void EndBlock()
        {
            throw new System.NotImplementedException();
        }


        public ClipTransition GetBlockAnimation()
        {
            throw new System.NotImplementedException();
        }


        public void StartBlock()
        {
            throw new System.NotImplementedException();
        }
    }


}
