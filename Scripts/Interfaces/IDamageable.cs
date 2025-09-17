

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
        
    }

}