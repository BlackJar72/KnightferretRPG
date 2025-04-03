using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace kfutils.rpg.ui {
    
    public class SpellRowUI : MonoBehaviour {

        private Spell spell01;
        [SerializeField] GameObject spellSlot01;
        [SerializeField] Image spellIcon01;
        [SerializeField] TMP_Text spellName01;
        public Spell Spell01 { get => spell01; }

        private Spell spell02;
        [SerializeField] GameObject spellSlot02;
        [SerializeField] Image spellIcon02;
        [SerializeField] TMP_Text spellName02;
        public Spell Spell02 { get => spell02; }


        public void Initialize() {
            spellSlot01.SetActive(false);
            spellSlot02.SetActive(false);
        }


        public void AddSpell(Spell spell, int slot) {
            slot %= 2;
            if(slot == 0) {
                spellSlot01.SetActive(true);
                spell01 = spell;
                spellIcon01.sprite = spell.Icon;
                spellName01.SetText(spell.Name);
            } else {
                spellSlot02.SetActive(true);
                spell01 = spell;
                spellIcon01.sprite = spell.Icon;
                spellName01.SetText(spell.Name);
            }
        }


    }

}