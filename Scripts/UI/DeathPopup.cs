using System.Collections;
using kfutils.rpg.ui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg
{


    public class DeathPopup : ShowOrHide
    {
        [SerializeField] GameObject reloadButton;
        [SerializeField] bool showReload = true;


        void OnEnable()
        {
            reloadButton.SetActive(showReload && !string.IsNullOrWhiteSpace(SavedGame.LastSave)
                                              && SavedGame.HasSave(SavedGame.LastSave));
        }


        /// <summary>
        /// Return to the main menu.  The player may reload an arbitrary save 
        /// or start a new game from there.  (Those options are not on the 
        /// pop-up due to space concernes.)
        /// </summary>
        public void MainMenu()
        {
            GameManager.Instance.UIManager.PlayButtonClick();
            SceneManager.LoadSceneAsync("StartScreen", LoadSceneMode.Single);
        }


        /// <summary>
        /// Reload the last save.  If the player has loaded a save and not 
        /// loaded since, it will reload that save.  If this is a new game 
        /// and no saves have been made this is not valid.
        /// </summary>
        public void Reload()
        {
            GameManager.Instance.UIManager.PlayButtonClick();
            if (!string.IsNullOrWhiteSpace(SavedGame.LastSave) && SavedGame.HasSave(SavedGame.LastSave))
            {
                GameManager.Instance.UIManager.ShowLoadingScreen();
                Time.timeScale = 0.0f; // FIXME: The pause should happen when the GUI is activated
                EntityManagement.playerCharacter.Inventory.Clear();
                StartCoroutine(LoadHelper());
            }
        }


        private IEnumerator LoadHelper()
        {
            yield return null;
            string fileToLoad = SavedGame.LastSave;
            Debug.Log(fileToLoad);
            SavedGame savedGame = new();
            savedGame.LoadWorld(fileToLoad);
            PCData pcData = savedGame.LoadPlayer(fileToLoad, EntityManagement.playerCharacter.GetPCData());
            EntityManagement.playerCharacter.SetPCData(pcData);
            yield return null;
            EntityManagement.playerCharacter.SetPCData(pcData);
            EntityManagement.playerCharacter.Inventory.OnEnable();
            EntityManagement.playerCharacter.Spells.OnEnable();
            InventoryManagement.SignalLoadNPCInventoryData();
            WorldManagement.SignalGameReloaded();
            GameManager.Instance.UIManager.HideLoadingScreen();
            SetHidden();
            Time.timeScale = 1.0f;
            InventoryManagement.SignalCloseUIs();
            GameManager.Instance.UIManager.CloseCharacterSheet();
            GameManager.Instance.UIManager.HidePauseMenu();
            EntityManagement.playerCharacter.AllowActions(true);
        }


    }


}