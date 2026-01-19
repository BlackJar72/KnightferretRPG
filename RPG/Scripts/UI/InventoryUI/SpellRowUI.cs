using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace kfutils.rpg.ui {
    
    public class SpellRowUI : MonoBehaviour {

        [SerializeField] SpellEntry spell01;
        public SpellEntry Spell01 { get => spell01; }
        [SerializeField] SpellEntry spell02;
        public SpellEntry Spell02 { get => spell02; }


        public void Initialize() {
            spell01.gameObject.SetActive(false);
            spell02.gameObject.SetActive(false);
        }


        public void AddSpell(Spell spell, int slot) {
            slot %= 2;
            if(slot == 0) {
                spell01.AddSpell(spell, slot);
            } else {
                spell02.AddSpell(spell, slot);
            }
        }


    }

}