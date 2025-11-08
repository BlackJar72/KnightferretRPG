using kfutils.rpg;
using UnityEngine;


namespace kfutils
{

    public class DamageArea : MonoBehaviour
    {
        [SerializeField] Collider area;
        [SerializeField] DamageType damageType;
        [SerializeField] int damage;
        [SerializeField] float armorPenetration = 1.0f;


        void OnTriggerStay(Collider other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>(); 
            if(damageable != null)
            {
                damageable.TakeDamageOverTime(DamageUtils.CalcFixedDamage(damage * GameConstants.ENVIRO_DMG_FACTOR,
                                              damageable.GetArmor(), armorPenetration, damageType));
            }
        }


    }

}
