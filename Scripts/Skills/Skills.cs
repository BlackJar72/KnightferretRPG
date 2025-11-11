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

        // Strength Skills (???) (possible addition: intimidation; may end up with no skills?!)

        // Agility Skills
        [SerializeField] Skill melee;
        [SerializeField] Skill missile;
        [SerializeField] Skill acrobatics;
        [SerializeField] Skill stealth;

        // Endurance Skills (few sills, but determines endurance)
        [SerializeField] Skill athletics;

        // Vitality Skills (???) (May end up with no skills, but still determines health / hit points)
        // Intelligence Skills (possible additions: crafting and/or smithng)
        [SerializeField] Skill thaumaturgy; // "Magical Theory," effect what spells can be learned (more?)
        [SerializeField] Skill spellcraft; // Effects the mana cost to cast spells
        [SerializeField] Skill security; 

        // Charisma skills (possible additions: entertaining, leadership)
        [SerializeField] Skill charm; // Effects relationshp increases and general reactions 
        [SerializeField] Skill persuasion; 

        // Spirit Skills (may end up with no skills, but still determines mana pool; possible addition: intimidation)
        [SerializeField] Skill theurgy; // Miracles, if I include them as distinct from magic (else, remove this)
        [SerializeField] Skill faith; // May not last, probably also related to miracles, if they exist (as a skill) 


        // Accessor form

        // Strength Skills (???)


        public Skill Melee => melee;
        public Skill Missile => missile;
        public Skill Acrobatics => acrobatics;
        public Skill Stealth => stealth;
        public Skill Athletics => athletics;
        public Skill Thaumaturgy => thaumaturgy;
        public Skill Spellcraft => spellcraft; 
        public Skill Security => security; 
        public Skill Charm => charm;
        public Skill Persuasion => persuasion;
        public Skill Theurgy => theurgy; 
        public Skill Faith => faith; 


    }

}
