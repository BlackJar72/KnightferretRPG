using UnityEngine;
using kfutils.rpg.ui;
using System.Collections.Generic;


namespace kfutils.rpg {
    
    public class Spellbook : MonoBehaviour, IInventory<SpellSlot> {
        
        public List<SpellSlot> spells;


        public int Count => spells.Count;
        public float Weight => 0.0f;


        public bool AddItemToSlot(int slot, SpellSlot item) {
            for(int i = 0; i < spells.Count; i++) {
                if(spells[i].slot == item.slot) return AddToFirstEmptySlot(item);
            }
            spells.Add(item);
            return true;
        }


        public bool AddToFirstEmptySlot(SpellSlot item) {
            bool found = false;
            int end = GetLastSlot() + 1;
            for(int i = 0; i < end; i++) {
                found = true;
                for(int j = 0; j < spells.Count; i++) {
                    found = found && (spells[j].slot != i);
                }
                if(found) {
                    item.slot = i;
                    spells.Add(item);
                    return true;
                }
            }
            item.slot = end;
            spells.Add(item);
            return true;
        }


        public bool AddToFirstReallyEmptySlot(SpellSlot item) {
            return AddToFirstEmptySlot(item);
        }


        public float CalculateWeight() => 0.0f;


        public SpellSlot GetByBackingIndex(int index) {
            return spells[index];
        }


        public SpellSlot GetItemInSlot(int slot) {
            for(int i = 0; i < spells.Count; i++) {
                if(spells[i].slot == slot) return spells[i];
            }
            return null;
        }


        public int GetLastSlot() {
            int result = 0;
            foreach(SpellSlot slot in spells) {
                if(slot.slot > result) result = slot.slot;
            }
            return result;
        }


        public int GetNumberInSlot(int slot) {
            for(int i = spells.Count - 1; i > -1; i--) {
                if(spells[i].slot == slot) {
                    spells.RemoveAt(i);
                    return 1;
                }
            }
            return 0;
        }


        public bool HasItem(SpellSlot item) {
            for(int i = 0; i < spells.Count; i++) {
                if(spells[i].spell == item.spell) return true;
            }
            return false;
        }


        public void RemoveAllFromSlot(int slot) {
            for(int i = spells.Count - 1; i > -1; i--) {
                if(spells[i].slot == slot) {
                    spells.RemoveAt(i);
                    return;
                }
            }
        }


        public void RemoveFromSlot(int slot, int number) {
            for(int i = spells.Count - 1; i > -1; i--) {
                if(spells[i].slot == slot) {
                    spells.RemoveAt(i);
                    return;
                }
            }
        }


        public void RemoveItem(SpellSlot item) {
            for(int i = spells.Count - 1; i > -1; i--) {
                if(spells[i].spell == item.spell) {
                    spells.RemoveAt(i);
                    return;
                }
            }
        }


        public void SignalSlotUpdate(int slot) {
            // TODO: Setup spellbook UI, then implement this
            throw new System.NotImplementedException();
        }


        public void SignalUpdate() {
            // TODO: Setup spellbook UI, then implement this
            throw new System.NotImplementedException();
        }


    }

}
