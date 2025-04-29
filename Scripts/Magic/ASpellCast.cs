using UnityEngine;

namespace kfutils.rpg {

    public abstract class ASpellCast : ScriptableObject, ISpellCast {

        [SerializeField] protected Spell spell;
        public abstract void Cast(IAttacker caster);


    }

}
