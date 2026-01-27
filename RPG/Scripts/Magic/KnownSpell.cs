using UnityEngine;


namespace kfutils.rpg {
    
    public class KnownSpell : IEquipable  
    {
        private readonly Spell spell;
        private bool isEquipped = false;
        private bool isOnHotbar = false;


        public KnownSpell(Spell spell) => this.spell = spell;



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

        public bool IsEquipped { get => isEquipped; set => isEquipped = value; }
        public bool IsOnHotbar { get => isOnHotbar; set => isOnHotbar = value; }
        public ItemPrototype Item { get => null; set => throw new System.NotImplementedException(); }
        public int StackSize { get => 1; set => throw new System.Exception(); }
        public int Slot { get => -1; set => throw new System.Exception(); }


        public void Equip() => isEquipped = true;
        public void UnEquip() => isEquipped = false;
        public void AddToHotbar() => isOnHotbar = true;
        public void RemoveFromHotbar() => isOnHotbar = false;



    }


}