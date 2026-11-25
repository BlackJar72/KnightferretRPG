using kfutils.rpg.ui;
using UnityEngine;


namespace kfutils.rpg {
    
    /// <summary>
    /// FIXME???  Not sure I should do it this way, it jsut put the relevant field data here -- that might actually be better.
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/Magic/Spell Prototype", fileName = "Spell", order = 10)]
    public class Spell : ScriptableObject, IInvReferant 
    {

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
        [SerializeField] int difficulty;
        [SerializeField] GameObject castParticles;
        [SerializeField] Sound castSound;

        private int numberID;

        // Accessor Properties
        public string ID { get => id; }
        public int NumberID { get => numberID; }
        public string Name { get => spellName; }
        public Sprite Icon {get => icon; } 
        public string Description { get => description; }
        public ESpellTypes CastType => castType;
        public int Difficulty => difficulty; 
        public ISpellCast SpellEffect => spellEffect;
        public int ManaCost => manaCost;
        public float Range => range;
        public float CastTime => castTime;
        public GameObject CastParticles => castParticles;
        public Sound CastSound => castSound; 



        public Spell MakeKnown() => Instantiate(this);


        public static bool operator ==(Spell a, Spell b) => (a is not null) && (b is not null) && a.ID.Equals(b.ID);
        public static bool operator !=(Spell a, Spell b) => (a is null) || (b is null) || !a.ID.Equals(b.ID);


        public override bool Equals(object other)
        {
            return (other is Spell) && (other as Spell).ID.Equals(ID);
        }


        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }


        // A separete method, to avoid accidental assignment
        public void SetNumberID(int id) {
            numberID = id;
        }


        //-----------------------------------------------------------------------------------------------------------//
        //                                     INVENTORY REFERANT CODE                                               //
        //-----------------------------------------------------------------------------------------------------------//
        #region INVENTORY REFERANT CODE 

        private int slot = -1;

        public int StackSize => 1;
        public int Slot => slot; // Not sure what to do here?  Current position in spellbook???

        public void SetSlot(int number) => slot = number;

        private IInvReferant.ReferantRemoval referantRemoval;
        // This is here because the interface requires it though not directly used; 
        // attempting to use this directly results in a compiler error flagged by the IDE. 
        event IInvReferant.ReferantRemoval IInvReferant.SignalReferantRemoval
        {
        add
            {
                referantRemoval += value;
            }

            remove
            {
                referantRemoval -= value;
            }
        }


        public void AddReference(IInvReference reference)
        {
            referantRemoval += reference.OnReferantRemoved;
        }


        public void RemoveReference(IInvReference reference)
        {            
            referantRemoval -= reference.OnReferantRemoved;
        }


        // Spells are knowlege and not just dropped, used up, or put away like 
        // items.  Therefore, this is treated as an error to warn the developer 
        // of possibly error. If a situation arrises where a spell needs to be 
        // removed, this can be edited or its effects created in another way. 
        [System.Obsolete]
        public void RemoveReferant()
        {
            Debug.LogError("Trying to call RemoveReferant() on KnownSpell; this should not normally happen");
            throw new System.Exception("RemoveReferant() was called on a KnownSpell!");
            //referantRemoval?.Invoke(this);
            //referantRemoval = null;
        }


        #endregion

    }

}
