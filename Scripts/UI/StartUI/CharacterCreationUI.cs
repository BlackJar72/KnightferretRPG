using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace kfutils.rpg {


    public class CharacterCreationUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField charcterName;
        [SerializeField] StatCreationUI statCreationUI;
        [SerializeField] Button confrimButton;
        [SerializeField] GameObject nameWarning;

 

        private void OnEnable()
        {
            StartNewCharacter();
        }


        public void StartNewCharacter()
        {
            Time.timeScale = 0.0f;
            statCreationUI.StartNewCharacter();
            charcterName.text = "";
        }


        private void OnDisable()
        {
            Time.timeScale = 1.0f;           
        }


        public void ConfirmCharacter()
        {
            if(string.IsNullOrWhiteSpace(charcterName.text))
            {
                DemandName();
                return;
            }
            else 
            {
                StartGame();
            }
        }



        //FIXME: Remove this and all references once a propper chracter creation system is in place
        private void StartGame()
        {
            GameManager.Instance.UI.PlayButtonClick();
            GameManager.Instance.CloseStartScreen();
            GameManager.Instance.InitializeNewPC();
            GameManager.Instance.CustomizeNewPC(charcterName.text, statCreationUI.Stats);
            GameManager.Instance.LoadStartingWorld();
        }


        public void DemandName()
        {
            nameWarning.SetActive(true);
            charcterName.Select();
        }


        public void BackToStartScreen()
        {
            GameManager.Instance.UI.PlayButtonClick();
            GameManager.Instance.EnterStartMenu();
        }



    }


}

