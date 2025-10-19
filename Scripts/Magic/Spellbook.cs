using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;


namespace kfutils.rpg
{

    public class Spellbook : MonoBehaviour, IInventory<Spell>
    {

        public List<Spell> spells;

        public bool unlocked = false; // Spells should not normally be removed from list; can make it possible in special circumstances
        public bool belongsToPC = false;


        public int Count => spells.Count;
        public float Weight => 0.0f;



        // For Testing; TODO??: Get rid of this, but add some other way to add starting gear ... or, do I need to...?
        [SerializeField] Spell[] startingItems;


        void Start() // FIXME: This will need to be moved to an method run only at the start of a new game!!
        {
            foreach (Spell spell in startingItems) AddToFirstEmptySlot(spell);
            SignalUpdate();
        }


        public bool AddItemToSlot(int slot, Spell spell)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i] == spell) return false; // Do not add a spell more than once
            }
            spells.Add(spell);
            SignalUpdate();
            return true;
        }


        public int AddToFirstEmptySlot(Spell spell)
        {
            AddItemToSlot(spells.Count, spell);
            return spells.Count - 1;
        }


        public int AddToFirstReallyEmptySlot(Spell spell)
        {
            AddItemToSlot(spells.Count, spell);
            return spells.Count - 1;
        }


        public float CalculateWeight() => 0.0f;


        public Spell GetByBackingIndex(int index)
        {
            return spells[index];
        }


        public Spell GetItemInSlot(int slot)
        {
            return GetByBackingIndex(slot);
        }


        public int GetLastSlot()
        {
            return spells.Count - 1;
        }


        public int GetNumberInSlot(int slot)
        {
            if (slot < spells.Count) return 1;
            return 0;
        }


        public bool HasItem(Spell spell)
        {
            for (int i = 0; i < spells.Count; i++)
            {
                if (spells[i] == spell) return true;
            }
            return false;
        }


        public void RemoveAllFromSlot(int slot)
        {
            if (unlocked)
            {
                spells.RemoveAt(slot);
                SignalUpdate();
            }
        }


        public void RemoveFromSlot(int slot, int number)
        {
            if (unlocked)
            {
                spells.RemoveAt(slot);
                SignalUpdate();
            }
        }


        public void RemoveItem(Spell spell)
        {
            if (unlocked)
            {
                for (int i = spells.Count - 1; i > -1; i--)
                {
                    if (spells[i] == spell)
                    {
                        spells.RemoveAt(i);
                        SignalUpdate();
                        return;
                    }
                }
            }
        }


        public virtual void SignalUpdate()
        {
            InventoryManagement.SignalSpellbookUpdate(this);
            CalculateWeight();
        }


        public virtual void SignalSlotUpdate(int slot)
        {
            InventoryManagement.SignalSpellbookUpdate(this);
            CalculateWeight();
        }


        public int FindFirstEmptySlot()
        {
            return spells.Count;
        }


        public int GetIndexOfSpell(Spell spell)
        {
            return spells.IndexOf(spell);
        }


        public bool BelongsToPC(IInventory<Spell> inv) => belongsToPC;





    }



}
