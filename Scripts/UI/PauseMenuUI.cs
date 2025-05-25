using UnityEngine;


namespace kfutils.rpg.ui
{


    public class PauseMenuUI : ShowOrHide
    {

        [SerializeField] SaveLoadUI saveLoadUI;


        public void SaveButtonClicked()
        {
            Time.timeScale = 0.0f;
            saveLoadUI.SetVisible();
            saveLoadUI.SetToSavePanel();
        }


        public void LoadButtonClicked()
        {
            Time.timeScale = 0.0f;
            saveLoadUI.SetVisible();
            saveLoadUI.SetToLoadPanel();
        }


        public void QuitButtonClicked() => Application.Quit();


        public void CancelButtonClicked()
        {
            SetHidden();
            Time.timeScale = 1.0f;
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
