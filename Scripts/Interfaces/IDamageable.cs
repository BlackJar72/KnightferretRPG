

namespace kfutils.rpg
{

    public class DamageData {
        public Damages damage;
        public ICombatant attacker;
        public DamageData(Damages damage, ICombatant attacker) {
            this.damage = damage;
            this.attacker = attacker;
        }
    }




    public interface IDamageable
    {

        public int GetArmor();
        public void TakeDamage(Damages damage);
        public void TakeDamage(DamageData damage);
        public EntityLiving GetEntity { get; }
        
    }

}