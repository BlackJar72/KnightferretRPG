

namespace kfutils.rpg
{

    public class DamageData {
        public Damages damage;
        public IAttacker attacker;
        public DamageData(Damages damage, IAttacker attacker) {
            this.damage = damage;
            this.attacker = attacker;
        }
    }




    public interface IDamageable {
        
        public int GetArmor();
        public void TakeDamage(Damages damage);
        public void TakeDamage(DamageData damage);
        
    }

}