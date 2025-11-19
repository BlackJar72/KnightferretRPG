using System.Collections;
using Animancer.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace kfutils.rpg.ui
{


    public class PauseMenuUI : ShowOrHide
    {

        [SerializeField] SaveLoadUI saveLoadUI;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip click;


        public void ShowMenu()
        {
            Time.timeScale = 0.0f;
            SetVisible();
        }


        private void PlayClick()
        {
            audioSource.clip = click;
            audioSource.Play();
        }


        public void SaveButtonClicked()
        {
            PlayClick();
            saveLoadUI.SetVisible();
            saveLoadUI.SetToSavePanel();
        }


        public void LoadButtonClicked()
        {
            PlayClick();
            saveLoadUI.SetVisible();
            saveLoadUI.SetToLoadPanel();
        }


        public void QuitButtonClicked()
        {
            PlayClick();
            Application.Quit();
        }


        public void CancelButtonClicked()
        {
            PlayClick();
            GameManager.Instance.UI.HidePauseMenu();
        }

        public void OptionsButtonClicked()
        {
            PlayClick();
            Debug.LogWarning("PauseMenuUI.OptionsButtonClicked() is not implemented yet!");
        }


        public void MainMenuButtonClicked()
        {
            PlayClick();
            GameManager.Instance.UI.HidePauseMenu();
            GameManager.Instance.EnterStartMenu();
        }



    }


}
