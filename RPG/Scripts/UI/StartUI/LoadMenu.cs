using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg.ui
{


    public class LoadMenu : MonoBehaviour
    {
        [SerializeField] StartMenu startMenu;
        [SerializeField] GameObject loadMenuItemPrefab;
        [SerializeField] GameObject loadContentArea;

        private List<LoadMenuButton> loadButtons = new();
        

        void OnEnable()
        {
            Redraw();
        }


        public void Redraw()
        {
            loadButtons.Clear();
            foreach (Transform child in loadContentArea.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < startMenu.SaveNames.Count; i++)
            {
                LoadMenuButton loadButton = Instantiate(loadMenuItemPrefab, loadContentArea.transform).GetComponent<LoadMenuButton>();
                loadButton.SetText(startMenu.SaveNames[i]);
                loadButton.SetOwner(this);
                loadButtons.Add(loadButton);
            }
        }


        public void SelectAsSave(LoadMenuButton loadButton)
        {
            startMenu.PlayShortUIClick();
            for (int i = 0; i < loadButtons.Count; i++)
            {
                loadButtons[i].SetSelected(loadButtons[i] == loadButton);
            }
            startMenu.SetFileToLoad(loadButton.GetText());
        }


        public void LoadSave()
        {
            startMenu.PlayUIClick();
            if (!string.IsNullOrWhiteSpace(startMenu.SaveToLoad)) startMenu.DoLoadGame();
        }


        public void DeleteSave()
        {
            startMenu.PlayUIClick();
            if (!string.IsNullOrWhiteSpace(startMenu.SaveToLoad))
            {
                SavedGame.DeleteSave(startMenu.SaveToLoad);
                if (PlayerPrefs.HasKey("LastSave") && PlayerPrefs.GetString("LastSave").Equals(startMenu.SaveToLoad))
                {
                    PlayerPrefs.DeleteKey("LastSave");
                }
                startMenu.InitSaveFiles();
                startMenu.ShowHideContinue();
                Redraw();
            }
        }
        

        
    }

}
