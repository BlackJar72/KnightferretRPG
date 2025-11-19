using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace kfutils.rpg.ui
{


    public class StartMenu : MonoBehaviour
    {
        [SerializeField] AudioClip click;
        [SerializeField] AudioClip shortClick;
        [SerializeField] AudioSource audioSource;
        [SerializeField] GameObject continueButton;
        [SerializeField] GameObject loadButton;
        [SerializeField] GameObject blockerPanel;
        [SerializeField] GameObject loadPanel;
        [SerializeField] LoadMenu loadMenu;

        [SerializeField] GameObject mainStartCanvas;
        [SerializeField] GameObject characterCreationCanvas; 

        private string lastSave = null;
        private string saveToLoad = null;
        private string[] files;
        private List<string> saveNames = new();

        public string SaveToLoad => saveToLoad;
        public List<string> SaveNames => saveNames;


        void Start()
        {
            ShowHideContinue();
            InitSaveFiles();
            blockerPanel.SetActive(false);
            loadPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        public void PlayUIClick()
        {
            audioSource.clip = click;
            audioSource.Play();
        }


        public void PlayShortUIClick()
        {
            audioSource.clip = shortClick;
            audioSource.Play();
        }


        /// <summary>
        /// Called by Start() to determine if the Continue button should be visible 
        /// based on if there is a legitimade previous save to load. 
        /// </summary>
        public void ShowHideContinue()
        {
            bool lastSaveExists = false;
            if (PlayerPrefs.HasKey("LastSave"))
            {
                lastSave = PlayerPrefs.GetString("LastSave");
                if (!string.IsNullOrWhiteSpace(lastSave))
                {
                    lastSaveExists = SavedGame.HasSave(lastSave);
                }
                // If the file was not valid, go ahead and remove it from player prefs
                if (!lastSaveExists)
                {
                    lastSave = null;
                    PlayerPrefs.DeleteKey("lastSave");
                }
            }
            continueButton.SetActive(lastSaveExists);
        }


        /// <summary>
        /// Called by the Start() to initialize lists of available save files that could be loaded. If 
        /// there are no save files available, it will also hide the load button. 
        /// </summary>
        public void InitSaveFiles()
        {
            saveNames.Clear();
            string folder = Application.persistentDataPath + Path.DirectorySeparatorChar + SavedGame.saveSubdir;
            files = Directory.GetFiles(folder);
            foreach (string filename in files)
            {
                if (filename.EndsWith(SavedGame.saveFileExtension))
                {
                    string[] parts = Path.GetFileNameWithoutExtension(filename).Split(Path.DirectorySeparatorChar);
                    string saveName = parts[parts.Length - 1];
                    saveNames.Add(saveName);
                }
            }
            loadButton.SetActive(saveNames.Count > 0);
        }


        /// <summary>
        /// Called from the New Game button on the UI. 
        /// 
        /// This should take the player to character creation scene.
        /// </summary>
        public void NewGame()
        {
            PlayUIClick();
            SavedGame.ClearLastSave();
            //FIXME: Don't do this, go to character creation instead
            GameManager.NewGame();
            //StartCoroutine(StartNewGame());
            //TODO: Go to character creation screen, once there is one
            characterCreationCanvas.SetActive(true);
            mainStartCanvas.SetActive(false);
        }


        /// <summary>
        /// Called by the continue button on the UI.
        /// 
        /// This will load the last save, skipping save solection
        /// </summary>
        public void ContinuePrevious()
        {
            if (string.IsNullOrWhiteSpace(lastSave))
            {
                Debug.LogError("Continue previous game was selected (public void ContinuePrevious()), but save path was invalid!");
#if UNITY_EDITOR
                throw new System.Exception("Continue previous game was selected (public void ContinuePrevious()), but save path was invalid!");
#endif
            }
            PlayUIClick();
            saveToLoad = lastSave;
            DoLoadGame();
        }


        /// <summary>
        /// Called by the Load Game button on the UI.
        /// 
        /// This will open a menu to select a save to load; this needs to have a list of 
        /// existing saves along with buttons to load the select save and exist the menu; 
        /// this cannot be the one used in game, as that includes a save menu.  A button 
        /// to delete the selected save should also exist.
        /// </summary>
        public void LoadGame()
        {
            PlayUIClick();
            blockerPanel.SetActive(true);
            loadPanel.SetActive(true);

        }


        public void HideLoadPanel()
        {
            PlayUIClick();
            blockerPanel.SetActive(false);
            loadPanel.SetActive(false);
        }


        public void SetFileToLoad(string filename)
        {
            saveToLoad = filename;
        }


        /// <summary>
        /// Called by the Setting button on the UI.
        /// 
        /// This should open the configuration UI. 
        /// </summary>
        public void OpenSettings()
        {
            Debug.Log("public void NewGame()");
            PlayUIClick();

        }


        /// <summary>
        /// Called by the Help/Info button on the UI.
        /// 
        /// This should be open help menus.  How these menus should be organized, how much 
        /// info they should hold (basic instrctions?  A full manual of some kind?), and 
        /// by extention if it should be panel, canvas, and separate scene, has not been 
        /// decided. 
        /// </summary>
        public void OpenHelpInfo()
        {
            Debug.Log("public void NewGame()");
            PlayUIClick();

        }


        /// <summary>
        /// This does the actual loading of the selected save file.  It is called by other 
        /// methods and menus/classes to do the actual work of loading the save.  It is 
        /// not tied directly to any UI element, only called indirectly through others. 
        /// </summary>
        public void DoLoadGame()
        {
            if (string.IsNullOrWhiteSpace(saveToLoad))
            {
                Debug.LogError("Trying to load save game from invalid path or non-existant file (saveToLoad)!  (public void DoLoadGame())");
#if UNITY_EDITOR
                throw new System.Exception("Trying to load save game from invalid path or non-existant file (saveToLoad)!  (public void DoLoadGame())");
#endif
            }
            GameManager.Instance.EnterPlayMode();
            GameManager.Instance.ConitnueLoading(saveToLoad);
        }


        /// <summary>
        /// Call by the Exit Game button on the UI.
        /// 
        /// Quit the games.
        /// </summary>
        public void ExitGame()
        {
            PlayUIClick();
            Application.Quit();
        }






    }


}
