using UnityEngine;
using kfutils.rpg.ui;
using UnityEngine.InputSystem;
using Unity.VisualScripting;



namespace kfutils.rpg {

    //[RequireComponent(typeof(PlayerInput))]
    public class UIManager : MonoBehaviour {

        // UI Control
        [SerializeField] ShowOrHide characterPanelToggler;
        [SerializeField] Canvas mainCanvas;
        [SerializeField] ShowOrHide containerUI;
        [SerializeField] ContainerUI containerLogic;


        public Canvas MainCanvas { get => mainCanvas; }


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
                InventoryManager.SignalCloseUIs();
            }
            return characterPanelToggler.IsVisible;
        }


        public void OpenContainerUI(Inventory inventory, Container container) {
            containerLogic.Initialize(inventory, container);
            containerUI.SetVisible();
        }


        public void CloseContainerUI() {
            containerUI.SetHidden();
        }
 

        public void ToggleContainerUI(Inventory inventory, Container container, GameObject from) {
            if(containerUI.gameObject.activeSelf) {
                CloseContainerUI();
                characterPanelToggler.SetHidden();
                Cursor.lockState = CursorLockMode.Locked;
                EntityManagement.playerCharacter.AllowActions(true);
            } else {
                OpenContainerUI(inventory, container);
                characterPanelToggler.SetVisible();
                Cursor.lockState = CursorLockMode.None;
                EntityManagement.playerCharacter.AllowActions(false);
            }
        }
 

    }

}
