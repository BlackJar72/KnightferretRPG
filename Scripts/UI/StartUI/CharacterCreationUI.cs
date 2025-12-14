using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

namespace kfutils.rpg {


    public class CharacterCreationUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField charcterName;
        [SerializeField] StatCreationUI statCreationUI;
        [SerializeField] Button confrimButton;
        [SerializeField] GameObject nameWarning;

#region External Control
        public delegate void CreatedCharacter();
        public static event CreatedCharacter CharacterCreationEvent;
        public delegate void BackToStart();
        public static event BackToStart BackToStartEvent;
        public delegate void NewCharacter();
        public static event NewCharacter NewCharacterEvent;
        
        private static bool characterCreated;
        public static bool CharacterCreated => characterCreated;
        /// <summary>
        /// Call this in any method subscribed to CharacterCreationEvent at which character creation 
        /// might fail or require further action, to tell if the external requirements have been met. 
        /// If any requirement has failed, this should stop the game from proceeding to starting 
        /// gameplay. In addition, a couple frames of delay will be provided for such methods to 
        /// run, do what they need to do, and return their result.
        /// </summary>
        /// <param name="success"></param>
        public static void SetCreationSuccess(bool success) => characterCreated &= success;
#endregion External Control

 

        private void OnEnable()
        {
            StartNewCharacter();
        }


        public void StartNewCharacter()
        {
            NewCharacterEvent?.Invoke();
            Time.timeScale = 0.0f;
            characterCreated = false;
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
                characterCreated = true;
                CharacterCreationEvent?.Invoke();
                StartCoroutine(StartIfCharacterCreated());
            }
        }


        private IEnumerator StartIfCharacterCreated()
        {
            yield return null;
            yield return null;
            if(characterCreated) StartGame();
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
            BackToStartEvent?.Invoke();
        }



    }


}

