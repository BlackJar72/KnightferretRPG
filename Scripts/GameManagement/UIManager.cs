using UnityEngine;
using kfutils.rpg.ui;
using UnityEngine.InputSystem;



namespace kfutils.rpg {

    public class UIManager : MonoBehaviour {

        // UI Control
        [SerializeField] ShowOrHide characterPanelToggler;
        [SerializeField] Canvas mainCanvas;
        [SerializeField] ShowOrHide crossHairs;
        [SerializeField] Toast toast;
        [SerializeField] ShowOrHide containerUI;
        [SerializeField] ContainerUI containerLogic;
        [SerializeField] ItemToolTipUI itemToolTipUI;


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


        /// <summary>
        /// Handles things that should be done when opening any GUI.
        /// </summary>
        public void OpenGUI() {
            Cursor.lockState = CursorLockMode.None;
            crossHairs.SetHidden();
        }


        /// <summary>
        /// Handles things that should be done when opening any GUI.
        /// </summary>
        public void CloseGUI() {                          
            Cursor.lockState = CursorLockMode.Locked;
            crossHairs.SetVisible();
        }


        public void ShowItemToolTip(ItemPrototype item) {
            itemToolTipUI.ShowToolTip(item);            
        }


        public void HideItemToolTip() {
            itemToolTipUI.HideToolTip();
        }


        public bool ToggleCharacterSheet() {
            characterPanelToggler.Toggle();
            if(characterPanelToggler.IsVisible) {
                OpenGUI();
            } else {
                CloseGUI();
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
                CloseGUI();
                CloseContainerUI();
                characterPanelToggler.SetHidden();
                Cursor.lockState = CursorLockMode.Locked;
                EntityManagement.playerCharacter.AllowActions(true);
            } else {
                OpenGUI();
                OpenContainerUI(inventory, container);
                characterPanelToggler.SetVisible();
                Cursor.lockState = CursorLockMode.None;
                EntityManagement.playerCharacter.AllowActions(false);
            }
        }

        
        public void ShowToast(string text) {
            toast.Show(text);
        }

        
        public void ShowToast(string text, float duration) {
            toast.Show(text, duration);
        }


    }

}
