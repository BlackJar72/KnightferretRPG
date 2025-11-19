using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {


    public class CharacterCreationUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField charcterName;
        [SerializeField] StatCreationUI statCreationUI;
        [SerializeField] Button confrimButton;


        private void OnEnable()
        {
            Time.timeScale = 0.0f;
        }


        private void Oisable()
        {
            Time.timeScale = 1.0f;           
        }


        public void ConfirmCharacter()
        {
            Debug.Log(charcterName.text);
            if(string.IsNullOrWhiteSpace(charcterName.text))
            {
                DemandName();
                return;
            }
            else 
            {
                StartCoroutine(StartGame());
            }
        }


        //FIXME: Remove this and all references once a propper chracter creation system is in place
        private System.Collections.IEnumerator StartGame()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneLoader", LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            yield return null;
            GameManager.Instance.InitializeNewPC();
            GameManager.Instance.CustomizeNewPC(charcterName.text, statCreationUI.Stats);
            GameManager.Instance.CloseStartScreen();
        }


        public void DemandName()
        {
            
        }



    }


}

