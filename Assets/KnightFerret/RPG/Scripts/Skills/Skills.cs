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
        //
        // FOR NOW THIS WILL BE UNUSED, THE GAME WILL USE STATS ONLY. THIS WILL BE KEPT FOR POSSIBLE 
        // FUTURE EXPANSION (I.E., FUTURE GAMES).  With skill, each level would get 5 skill points, 
        // with each new add costing a number of points equal to that add, while a stat could be increased 
        // every three levels and the maximum level would be relatively high.  However, in the current 
        // plan there will be no skills, a stat will be able to be increased with each level, and the
        // maximum expected level to be typically obtained will be relatively low (sady 10 or 12, or less). 
        */

        // Strength Skills (???) (possible additions: climbing, intimidation; may end up with no skills?!)

        // Agility Skills  (possible additions: dodge/defence)
        [SerializeField] Skill melee;
        [SerializeField] Skill missile;
        [SerializeField] Skill acrobatics;
        [SerializeField] Skill stealth;

        // Endurance Skills (few sills, but determines endurance)
        [SerializeField] Skill athletics;

        // Vitality Skills (???) (May end up with no skills, but still determines health / hit points)

        // Intelligence Skills (possible additions: crafting skills (Crafting? Smithng? Alchemy?), Linguistics
        [SerializeField] Skill thaumaturgy; // "Magical Theory," effect what spells can be learned (more?)
        [SerializeField] Skill security; 

        // Willpower Skills 
        [SerializeField] Skill spellcraft; // Effects the mana cost to cast spells

        // Charisma skills (possible additions: Entertaining/Performance/Song, Leadership)
        [SerializeField] Skill charm; // Effects relationshp increases and general reactions 
        [SerializeField] Skill persuasion; 

        // Spirit Skills (may end up with no skills, but still determines mana pool; possible addition: intimidation)
        [SerializeField] Skill theurgy; // Miracles, if I include them as distinct from magic (else, remove this)
        [SerializeField] Skill faith;  // May not last, probably also related to miracles, if they exist (as a skill) 


        /*
        // Skill ideas not listed or listed in comments above and possible but not yet decided upon:
        //
        // Intimidation: Standard use, but to base it on strength (tough / muscular appearance) or spirit (creating presence)?
        //             Not considering Charisma (also presence in a different way) because that already has persuasion. 
        // Entertaining, Performance, or Song: Produce a minor charm-like effect, increasing relationship at modest range without talking 
        //             (and/or possibly a chance to pacify enemies) 
        // Leadership: Determines max number of followers (number being (Leadership + 5) / 10 rounded down to integeer); below 5, no followers! 
        //             This will probably not be used as I'm not sure I'll even have a follower system, or a big enough game to contain many 
        //             potential followers anyway.
        // Crafting, Smithing, and/or Alchemy: Use to create some items like in many existing games
        // Climbing: Scale shear surfaces such as walls, like in Daggerfall (or like a thief in early D&D)
        // Defence: Dodging and keeping gaps in any armor safe (improves armor rating, like agility without skills butt better)
        // Linugistics: Read ancient or obscure languages (like the thief skill in early D&D)
        */

        /*
        // Likely / expected relation to skill and common classes / architypes:
        //
        // Melee + Missile: Fighter / Warrior 
        // Thaumaturgy + Spellcraft: Mage
        // Stealth + Secrutiy: Thief / Rogue
        // Acrobatics + Atheletics: Acrobat, or Adventurer/Explorer (especially one who like raiding tombs)
        // Faith + Theurgy + Melee: Cleric (or Paladin if melee is the main focus)
        // Performance + combat, magic, or social skill: Some version of a Bard 
        // Charm + Persuasion: Diplomatic / Negotiator / "Talky" character 
        // Stealth + Melee and/or Missile: Assassin or Ranger
        //
        // This is not intended to be a class based game, but these seems like obvious ways som common  
        // character classes and similar architypes could be represented.  The play should be able to 
        // create the character they want, with or without closely mirroring and exist trope. Characters 
        // will not have a defined class (but including optional archetypes as short-cut in character 
        // creation could work?).
        */


        // Accessor form

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
