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

        public void DoDamage(IDamageable victim, float factor = 1.0f) {
            victim.TakeDamage(DamageUtils.CalcDamage(Mathf.RoundToInt(baseDamage * factor), victim.GetArmor(), damageType, armorPenetration));
        }

        public DamageData GetDamage(ICombatant attacker, IWeapon weapon, int armor, float factor = 1.0f) {
            return new DamageData(DamageUtils.CalcDamage(Mathf.RoundToInt(baseDamage * factor), armor, damageType, armorPenetration), attacker, weapon);
        }

        public DamageData GetDamage(ICombatant attacker, IWeapon weapon, IDamageable victim, float factor = 1.0f) {
            return new DamageData(DamageUtils.CalcDamage(Mathf.RoundToInt(baseDamage * factor), victim.GetArmor(), damageType, armorPenetration), attacker, weapon);
        }

        public void DoDamage(ICombatant attacker, IWeapon weapon, IDamageable victim, float factor = 1.0f) {
            victim.TakeDamage(new DamageData(DamageUtils.CalcDamage(Mathf.RoundToInt(baseDamage * factor), victim.GetArmor(), damageType, armorPenetration), attacker, weapon));
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

        public DamageData GetDamageI(ICombatant attacker, int armor, IWeapon weapon) {
            return damageSource.GetDamage(attacker, weapon, armor);
        }

        public void DoDamage(ICombatant attacker, IDamageable victim, IWeapon weapon) {
            damageSource.DoDamage(attacker, weapon, victim);
        }
    }



}