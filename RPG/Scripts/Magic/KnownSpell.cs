using kfutils.rpg.ui;
using UnityEngine;

namespace kfutils.rpg {

    [System.Serializable]
    public class KnownSpell : IInvReferant 
    {
        [SerializeField] Spell spell;

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
        public Spell GetSpell() => spell;


        public static bool operator ==(KnownSpell a, KnownSpell b) => a.spell == b.spell;
        public static bool operator !=(KnownSpell a, KnownSpell b) => a.spell != b.spell;


        public KnownSpell(Spell spell)
        {
            this.spell = spell;
        }


        //-----------------------------------------------------------------------------------------------------------//
        //                                     INVENTORY REFERANT CODE                                               //
        //-----------------------------------------------------------------------------------------------------------//
        #region INVENTORY REFERANT CODE 


        public int StackSize => 1;
        public int Slot => throw new System.NotImplementedException(); // Not sure what to do here?  Current position in spellbook???


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


        public override bool Equals(object obj)
        {
            return this == (KnownSpell)obj;
        }


        public override int GetHashCode()
        {
            return spell.GetHashCode();
        }



    }


}
