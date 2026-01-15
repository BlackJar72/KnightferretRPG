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
            SetHidden();
            GameManager.Instance.UI.PlayButtonClick();
            GameManager.Instance.EnterStartMenu();
        }


        public void Quit()
        {
            Application.Quit();
        }


        /// <summary>
        /// Reload the last save.  If the player has loaded a save and not 
        /// loaded since, it will reload that save.  If this is a new game 
        /// and no saves have been made this is not valid.
        /// </summary>
        public void Reload()
        {
            GameManager.Instance.UI.PlayButtonClick();
            if (!string.IsNullOrWhiteSpace(SavedGame.LastSave) && SavedGame.HasSave(SavedGame.LastSave))
            {
                GameManager.Instance.UI.ShowLoadingScreen();
                EntityManagement.playerCharacter.Inventory.Clear();
                GameManager.Instance.ConitnueLoading(SavedGame.LastSave);
                SetHidden();
            }
        }
        


    }


}