using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;


namespace kfutils.rpg
{


    [System.Serializable]
    public class SpellbookData
    {
        protected readonly string id;
        public List<Spell> inventory;
        public string ID => id;
        public SpellbookData(Spellbook inv)
        {
            id = inv.ID;
            inventory = inv.Spells;
        }
    }
    

    public class Spellbook : MonoBehaviour, IInventory<Spell>, ISerializationCallbackReceiver
    {

        [SerializeField] protected string id;

        [SerializeField] List<Spell> spells;

        [SerializeField] bool unlocked = false; // Spells should not normally be removed from list; can make it possible in special circumstances
        [SerializeField] bool belongsToPC = false;
        [SerializeField] Spell[] startingItems;


        public int Count => spells.Count;
        public float Weight => 0.0f;

        public bool Unlocked => unlocked;
        public bool OwnedByPC => belongsToPC;
        public List<Spell> Spells => spells;
        public string ID => id;

        public void SetID(string ID) => id ??= ID;
        public void Lock() => unlocked = false;
        public void Unlock() => unlocked = true;

        private bool initialized = false;
        public bool Initialized => initialized;


        public virtual void OnEnable()
        {
            SpellbookData data = InventoryManagement.GetSpellbookData(id);
            if (data == null)
            {
                data = new(this);
                foreach (Spell spell in startingItems) AddToFirstEmptySlot(spell);
                InventoryManagement.StoreSpellbookData(data);
            }
            else
            {
                spells = data.inventory;
            }
            SignalUpdate();
            initialized = true;
        }


        public void AddedStartingGear(InitialPCData initialData)
        {
            initialData.AddSpellsToBook(this);
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


        public void Clear()
        {
            spells.Clear();
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


        public static bool BelongsToPC(IInventory<Spell> inv) => inv.OwnedByPC;


        public void OnBeforeSerialize()
        {
            IActor owner = GetComponent<IActor>();
            if (owner != null) id = owner.ID;
        }


        public void OnAfterDeserialize() { }
        

    }



}
