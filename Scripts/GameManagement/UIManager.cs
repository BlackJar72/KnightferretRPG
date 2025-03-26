using UnityEngine;
using kfutils.rpg.ui;
using UnityEngine.InputSystem;



namespace kfutils.rpg {

    //[RequireComponent(typeof(PlayerInput))]
    public class UIManager : MonoBehaviour {

        // UI Control
        [SerializeField] ShowOrHide characterPanelToggler;


        // Input System
        // HELP!  Do I really need any of this?!
        protected PlayerInput input;
        protected PlayerInput submit;
        protected InputAction cancel;
        protected InputAction leftClick;
        protected InputAction middleClick;
        protected InputAction rightClick;
        protected InputAction sprintToggle;
        protected InputAction crouchAction;
        protected InputAction crouchToggle;


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
