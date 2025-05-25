using UnityEngine;
using kfutils.rpg.ui;



namespace kfutils.rpg {

    public class UIManager : MonoBehaviour {

        // UI Control
        [SerializeField] AudioSource buttonClick;
        [SerializeField] ShowOrHide characterPanelToggler;
        [SerializeField] Canvas mainCanvas;
        [SerializeField] ShowOrHide crossHairs;
        [SerializeField] Toast toast;
        [SerializeField] ShowOrHide containerUI;
        [SerializeField] ContainerUI containerLogic;
        [SerializeField] ItemToolTipUI itemToolTipUI;
        [SerializeField] ItemStackManipulator itemStackManipulator;
        [SerializeField] EquiptmentPanel pcEquiptPanel;
        [SerializeField] SaveLoadUI saveLoadPanel;
        [SerializeField] PauseMenuUI pauseMenuUI;


        public EquiptmentPanel PlayerEquiptPanel => pcEquiptPanel;


        public Canvas MainCanvas { get => mainCanvas; }


        private bool pauseMenuVisible = false;


        public bool PauseMenuVisible => pauseMenuVisible;


        /// <summary>
        /// Handles things that should be done when opening any GUI.
        /// </summary>
        public void OpenGUI()
        {
            Cursor.lockState = CursorLockMode.None;
            crossHairs.SetHidden();
        }


        /// <summary>
        /// Handles things that should be done when opening any GUI.
        /// </summary>
        public void CloseGUI() {
            if (!pauseMenuVisible)
            {
                Cursor.lockState = CursorLockMode.Locked;
                crossHairs.SetVisible();
            }
        }


        public void PlayButtonClick()
        {
            buttonClick.Play();
        }


        public void ShowItemToolTip(ItemPrototype item)
        {
            if (!itemStackManipulator.IsVisible) itemToolTipUI.ShowToolTip(item);
        }


        public void ShowSpellToolTip(Spell spell) {
            if(!itemStackManipulator.IsVisible) itemToolTipUI.ShowSpellToolTip(spell);            
        }


        public void HideItemToolTip() {
            itemToolTipUI.HideToolTip();
        }


        public void ShowItemStackManipulator(InventorySlotUI slot) {
            if(slot is not EquipmentSlotUI) {
                itemStackManipulator.Open(slot);
                itemToolTipUI.HideToolTip();
            }
        }


        public void HideItemStackManipulator() {
            itemStackManipulator.Close();
        }


        public bool IsContainerUIVisible => containerUI.IsVisible;


        public bool ToggleSaveMenu(bool saveing = true) {
            saveLoadPanel.Toggle();
            if (saveing) saveLoadPanel.SetToSavePanel();
            else saveLoadPanel.SetToLoadPanel();
            if (saveLoadPanel.IsVisible || pauseMenuVisible) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
            return saveLoadPanel.IsVisible;
        }


        public bool HideSaveMenu() {
            saveLoadPanel.SetHidden();
            return characterPanelToggler.IsVisible;
        }


        public void PauseButtonHit()
        {
            if (pauseMenuVisible || characterPanelToggler.IsVisible)
            {
                CloseCharacterSheet();
                HidePauseMenu();
            }
            else ShowPauseMenu();
        }


        public void TooglePauseMenu()
        {
            if (pauseMenuVisible) HidePauseMenu();
            else ShowPauseMenu();
        }
        

        public void ShowPauseMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            EntityManagement.playerCharacter.AllowActions(false);
            pauseMenuUI.ShowMenu();
            pauseMenuVisible = true;
        }


        public void HidePauseMenu()
        {
            Cursor.lockState = CursorLockMode.Locked;
            EntityManagement.playerCharacter.AllowActions(true);
            pauseMenuVisible = false;
            HideSaveMenu();
            pauseMenuUI.SetHidden();
        }


        public bool ToggleCharacterSheet()
        {
            characterPanelToggler.Toggle();
            if (characterPanelToggler.IsVisible)
            {
                OpenGUI();
            }
            else
            {
                CloseGUI();
                CloseContainerUI();
                HideItemToolTip();
                HideItemStackManipulator();
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


        public void CloseCharacterSheet()
        {
            characterPanelToggler.SetHidden();
            CloseGUI();
            CloseContainerUI();
            HideItemToolTip();
            HideItemStackManipulator();
        }


        public bool CharacterSheetVisible => characterPanelToggler.IsVisible;
 

        public void ToggleContainerUI(Inventory inventory, Container container, GameObject from)
        {
            if (containerUI.IsVisible)
            {
                CloseGUI();
                CloseContainerUI();
                characterPanelToggler.SetHidden();
                Cursor.lockState = CursorLockMode.Locked;
                EntityManagement.playerCharacter.AllowActions(true);
            }
            else
            {
                OpenGUI();
                OpenContainerUI(inventory, container);
                characterPanelToggler.SetVisible();
                if (!pauseMenuVisible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    EntityManagement.playerCharacter.AllowActions(false);
                }
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
