using UnityEngine;

namespace kfutils.rpg {

    public abstract class ASpellCast : ScriptableObject, ISpellCast
    {
        public abstract void Cast(IActor caster);


    }

}
