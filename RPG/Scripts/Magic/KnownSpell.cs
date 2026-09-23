using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public class KnownSpell : IInvReferant 
    {
        // This will be public, and importanatly also not readonly, 
        // to fascilitate easy copying and serialization.  Having this 
        // private and readonly broke it before.  We are now using 
        // the Python "consenting adults" philosophy; just don't 
        // change / write to this! 
        public Spell spell; 

        // Accessor Properties
        public string ID { get => spell.ID; }
        public string Name { get => spell.Name; }
        public Sprite Icon {get => spell.Icon; } 
        public string Description { get => spell.Description; }
        public ESpellTypes CastType => spell.CastType;
        public int Difficulty => spell.Difficulty; 
        public ISpellCast SpellEffect => spell.SpellEffect;
        public int ManaCost => spell.ManaCost;
        public float Range => spell.Range;
        public float CastTime => spell.CastTime;
        public GameObject CastParticles => spell.CastParticles;
        public Sound CastSound => spell.CastSound; 


        public static bool operator ==(KnownSpell a, KnownSpell b) 
                    => (a is not null) && (b is not null) && (a.spell == b.spell);
        public static bool operator !=(KnownSpell a, KnownSpell b) 
                    => (a is null) || (b is null) || (a.spell != b.spell);


        public KnownSpell(Spell spell) => this.spell = spell;


        public override bool Equals(object obj)
        {
            return (obj is KnownSpell other) && (other.spell == spell);
        }


        public override int GetHashCode()
        {
            if(spell == null)
            {
                Debug.LogError("KnownSpell.spell is null! (This should not happen.)");
                return base.GetHashCode();
            }
            return spell.GetHashCode();
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


        public void AddReference(ui.IInvReference reference)
        {
            referantRemoval += reference.OnReferantRemoved;
        }


        public void RemoveReference(ui.IInvReference reference)
        {            
            referantRemoval -= reference.OnReferantRemoved;
        }


        // Spells are knowlege and not just dropped, used up, or put away like 
        // items.  Therefore, this is treated as an error to warn the developer 
        // of possible error. If a situation arrises where a spell needs to be 
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
