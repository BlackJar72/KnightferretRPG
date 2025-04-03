using UnityEngine;


namespace kfutils.rpg {
    
    public class CharacterPanelContols : MonoBehaviour {

        [SerializeField] GameObject inventoryPanel;
        [SerializeField] GameObject spellbookPanel;
        

        public void ShowInventory() {
            inventoryPanel.SetActive(true);
            spellbookPanel.SetActive(false);
        }
        

        public void ShowISpellbook() {
            inventoryPanel.SetActive(false);
            spellbookPanel.SetActive(true);
        }

    }


}
