using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public struct DamageSource {
        [SerializeField] int baseDamage;
        [SerializeField] DamageType damageType;
        [Range(0,1)]
        [SerializeField] float armorPenetration;
        public int BaseDamage => baseDamage;
        public DamageType Type => damageType;

        public Damages GetDamage(int armor) {
            return DamageUtils.CalcDamage(baseDamage, armor, damageType, armorPenetration);
        }

        public void DoDamage(IDamageable victim) {
            victim.TakeDamage(DamageUtils.CalcDamage(baseDamage, victim.GetArmor(), damageType, armorPenetration));
        }

        public DamageData GetDamage(IAttacker attacker, int armor) {
            return new DamageData(DamageUtils.CalcDamage(baseDamage, armor, damageType, armorPenetration), attacker);
        }

        public void DoDamage(IAttacker attacker, IDamageable victim) {
            victim.TakeDamage(new DamageData(DamageUtils.CalcDamage(baseDamage, victim.GetArmor(), damageType, armorPenetration), attacker));
        }
    }


    [CreateAssetMenu(menuName = "KF-RPG/Combat/Damage Source", fileName = "Damage", order = 10)]
    public class DamageSourceObj : ScriptableObject {
        [SerializeField] DamageSource damageSource;

        public Damages GetDamage(int armor) {
            return damageSource.GetDamage(armor);
        }

        public void DoDamage(IDamageable victim) {
            damageSource.DoDamage(victim);
        }

        public DamageData GetDamageI(IAttacker attacker, int armor) {
            return damageSource.GetDamage(attacker, armor);
        }

        public void DoDamage(IAttacker attacker, IDamageable victim) {
            damageSource.DoDamage(attacker, victim);
        }
    }



}