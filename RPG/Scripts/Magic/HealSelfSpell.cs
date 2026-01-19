using UnityEngine;

namespace kfutils.rpg
{


    [CreateAssetMenu(menuName = "KF-RPG/Magic/Heal Self Spell", fileName = "HealSelfCast", order = 30)]
    public class HealSelfSpell : ASpellCast
    {
        [SerializeField] int amount;

        public override void Cast(ICombatant caster)
        {
            caster.HealDamage(amount);
        }


    }

}
