using UnityEngine;


namespace kfutils.rpg {

    [System.Serializable]
    public enum ESpellTypes {

        // Spell effects caster
        SELF, 
        // Spell works within interactive range
        TOUCH, 
        // Spell is ranged and works by spawning a projectile
        PROJECTILE,
        // Spell is ranged and works by casting a ray; the most complex type as a ray should by 
        // fired to produce a marker special effect (for targetting) when cast button is down, then 
        // produce its targetted effect when the button is release.  A good example of a spell that 
        // would use this is a short-range teleport (planned name, translocation) where the caster 
        // would teleport to the location. 
        RANGED,
        // A Spell that produces an effect on the caster as long as the cast key is held down
        CONTIUOUS_SELF,
        // A spell the produces cojures or projecs someting as long and the cast key is held down
        CONTIUOUS_RANGED

    }

}
