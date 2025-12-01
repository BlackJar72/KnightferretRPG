using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Transforms;
using Unity.VisualScripting;

namespace kfutils.rpg {


    public class CharacterCreationUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField charcterName;
        [SerializeField] StatCreationUI statCreationUI;
        [SerializeField] Button confrimButton;
        [SerializeField] GameObject nameWarning;
        [SerializeField] GameObject avatarCreationPrefab;
        [SerializeField] GameObject avatarCreationPanel;

 

        private void OnEnable()
        {
            StartNewCharacter();
            SpawnAvatarCreation();
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


        public void SpawnAvatarCreation()
        {
            if((avatarCreationPrefab != null) && (avatarCreationPanel.transform.childCount < 1)) 
            {
                GameObject Child = Instantiate(avatarCreationPrefab, avatarCreationPanel.transform);
                avatarCreationPanel.SetActive(true);
                RectTransform rect = transform as RectTransform;
                RectTransform childRect = Child.transform as RectTransform;
                if((rect == null) || (childRect == null))
                {
                    Destroy(Child);
                    return;
                }
                childRect.anchoredPosition = Vector2.zero;
                childRect.anchorMax.Set(1, 1);
                childRect.anchorMin.Set(0, 0);
                childRect.offsetMin.Set(0, 0);
                childRect.offsetMax.Set(0, 0);
                //childRect.sizeDelta = Vector2.zero;
            }
            else if(avatarCreationPanel.transform.childCount < 1) avatarCreationPanel.SetActive(false);
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

