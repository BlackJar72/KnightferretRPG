using Animancer.Examples;
using UnityEngine;


namespace kfutils.rpg.ui
{


    public class PauseMenuUI : ShowOrHide
    {

        [SerializeField] SaveLoadUI saveLoadUI;


        public void ShowMenu()
        {
            Time.timeScale = 0.0f;
            SetVisible();
        }


        public void SaveButtonClicked()
        {
            saveLoadUI.SetVisible();
            saveLoadUI.SetToSavePanel();
        }


        public void LoadButtonClicked()
        {
            saveLoadUI.SetVisible();
            saveLoadUI.SetToLoadPanel();
        }


        public void QuitButtonClicked() => Application.Quit();


        public void CancelButtonClicked()
        {
            GameManager.Instance.UIManager.HidePauseMenu();
        }


        public void OptionsButtonClicked()
        {
            Debug.LogWarning("PauseMenuUI.OptionsButtonClicked() is not implemented yet!");
        }


        public void MainMenuButtonClicked()
        {
            Debug.LogWarning("PauseMenuUI.MainMenuButtonClicked() is not implemented yet!");
        }



    }


}
