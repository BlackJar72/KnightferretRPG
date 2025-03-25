using UnityEngine;
using kfutils.rpg.ui;



namespace kfutils.rpg {

    public class UIManager : MonoBehaviour {
        // UI Control
        [SerializeField] ShowOrHide characterPanelToggler;



        public bool ToggleCharacterSheet() {
            characterPanelToggler.Toggle();
            if(characterPanelToggler.IsVisible) {
                Cursor.lockState = CursorLockMode.None;
            } else {                          
                Cursor.lockState = CursorLockMode.Locked;
            }
            return characterPanelToggler.IsVisible;
        }


    }

}
