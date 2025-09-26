

namespace kfutils.rpg
{

    public class DamageData {
        public Damages damage;
        public ICombatant attacker;
        public IWeapon weapon;
        public DamageData(Damages damage, ICombatant attacker, IWeapon weapon)
        {
            this.damage = damage;
            this.attacker = attacker;
            this.weapon = weapon;
        }
    }




    public interface IDamageable
    {

        public int GetArmor();
        public void TakeDamage(Damages damage);
        public void TakeDamage(DamageData damage);
        public EntityLiving GetEntity { get; }
        public Damages ApplyDamageAdjustment(Damages damage);


        /// <summary>
        /// It the combatant current stunned?
        /// </summary>
        /// <returns></returns>
        public bool IsStunned();


        /// <summary>
        /// Has the combatant just been parried?
        /// </summary>
        /// <returns></returns>
        public bool InParriedState();


        /// <summary>
        /// Set the combatant to be in a parried state.
        /// </summary>
        /// <param name="parried"></param>
        public void SetParried(bool parried = true);


        public bool IsSurprised(ICombatant attacker);
        
    }

}