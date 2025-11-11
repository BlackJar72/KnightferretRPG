using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class Skills
    {
        /*
        // A PRELIMINARY LIST, VERY SUBJECT TO CHANGE, hopefully to some expansion and balancing 
        // (Might also need to expand stats, possible candidates being willpower and dexterity) 
        //
        // This whole system might need to overhauled or re-written, but it seemed like a good 
        // idea to start getting some ideas into a more concrete form.  (This is quasi-brainstorming.)
        */

        // Strength Skills (???)

        // Agility Skills
        [SerializeField] Skill melee;
        [SerializeField] Skill missile;
        [SerializeField] Skill acrobatics;
        [SerializeField] Skill stealth;

        // Endurance Skills 
        [SerializeField] Skill athletics;

        // Vitality Skills (???) 
        // Intelligence Skills
        [SerializeField] Skill thaumaturgy; // "Magical Theory," effect what spells can be learned (more?)
        [SerializeField] Skill spellcraft; // Effects the mana cost to cast spells

        // Charisma skills
        [SerializeField] Skill charm;
        [SerializeField] Skill persuasion;

        // Spirit Skills 
        [SerializeField] Skill theurgy; // Miracles, if I include them as distinct from magic (else, remove this)
        [SerializeField] Skill faith; // May not last, probably also related to miracles, if they exist (as a skill) 


    }

}
