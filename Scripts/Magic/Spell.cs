using UnityEngine;


namespace kfutils.rpg {
    
    /// <summary>
    /// FIXME???  Not sure I should do it this way, it jsut put the relevant field data here -- that might actually be better.
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/Magic/Spell Prototype", fileName = "Spell", order = 10)]
    public class Spell : ScriptableObject {


        public const EEquiptSlot equiptType = EEquiptSlot.SPELL; // Violating usual formating standards to mirror ItemPrototype
        
        // Identifying Data
        [SerializeField] string id;
        [SerializeField] string spellName;
        [SerializeField] Sprite icon;
        [SerializeField] string description;

        //Technical Data
        [SerializeField] ESpellTypes castType;
        [SerializeField] ASpellCast spellEffect;
        [SerializeField] int manaCost;
        [SerializeField] float range;
        [SerializeField] float castTime;
        [SerializeField] GameObject castParticles;
        [SerializeField] Sound castSound;



        // Accessor Properties
        public string ID { get => id; }
        public string Name { get => spellName; }
        public Sprite Icon {get => icon; } 
        public string Description { get => description; }
        public ESpellTypes CastType => castType;
        public ISpellCast SpellEffect => spellEffect;
        public int ManaCost => manaCost;
        public float Range => range;
        public float CastTime => castTime;
        public GameObject CastParticles => castParticles;
        public Sound CastSound => castSound; 


        

    }

}
