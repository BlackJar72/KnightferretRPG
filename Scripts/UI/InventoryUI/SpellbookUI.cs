using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace kfutils.rpg.ui {

    public class SpellbookUI : MonoBehaviour, IRedrawing  {

        [SerializeField] protected SpellRowUI slotPrefab = null;
        [SerializeField] protected Spellbook spellbook;
        [SerializeField] GameObject noSpellTxt;

        private List<SpellRowUI> rows =  new();



        protected virtual void OnEnable() {
            InventoryManager.spellbookUpdated += UpdateInventory;
            Redraw();
        }


        protected virtual void OnDisable() {
            InventoryManager.spellbookUpdated -= UpdateInventory;
        }


        public void Redraw() {
            InventoryManager.AddRedraw(this);
        }


        public void DoRedraw() {
            rows.Clear();
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            int numSpells = spellbook.Count;
            noSpellTxt.SetActive(numSpells < 1);
            int numRows = (numSpells / 2) + (numSpells % 2);
            for(int i = 0; i < numRows; i++) {
                SpellRowUI row = Instantiate(slotPrefab, transform);
                row.Initialize();
                rows.Add(row);
            }
            for(int i = 0; i < spellbook.Count; i++) {
                rows[i / 2].AddSpell(spellbook.spells[i], i);
            }

        }


        protected void UpdateInventory(Spellbook inv) {
            if(inv == spellbook) Redraw();
        }

    }

}