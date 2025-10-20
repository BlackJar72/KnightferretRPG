using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui
{


    public class SaveLoadUI : MonoBehaviour
    {

        [SerializeField] ShowOrHide shower;
        [SerializeField] GameObject saveMenuItemPrefab;
        [SerializeField] GameObject loadMenuItemPrefab;

        [SerializeField] GameObject savePanel;
        [SerializeField] GameObject saveContentArea;
        [SerializeField] GameObject loadPanel;
        [SerializeField] GameObject loadContentArea;

        [SerializeField] Image saveButtonImg;
        [SerializeField] Image loadButtonImg;

        [SerializeField] Sprite buttonOn;
        [SerializeField] Sprite buttonOff;

        [SerializeField] GameObject loadingScreen;


        private string[] files;
        private List<string> saveNames = new();
        private List<string> loadNames = new();
        private string fileToSave = null;
        private string fileToLoad = null;

        private readonly char[] ForbidenChars = new char[] {'"', '\'', '\t', '\r', '\n',
            Path.PathSeparator, Path.VolumeSeparatorChar, Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }; 

        private List<SaveButtonUI> saveButtons = new();
        private List<LoadButtonUI> loadButtons = new();

        public bool IsVisible => shower.IsVisible;


        void OnEnable()
        {
            fileToLoad = fileToSave = null; // FIXME/TODO: This should default to the last save used (or a new save if first in a new game)
            saveNames.Clear();
            loadNames.Clear();
            string folder = Application.persistentDataPath + Path.DirectorySeparatorChar + SavedGame.saveSubdir;
            files = Directory.GetFiles(folder);
            foreach (string filename in files)
            {
                if (filename.EndsWith(SavedGame.saveFileExtension))
                {
                    string[] parts = Path.GetFileNameWithoutExtension(filename).Split(Path.DirectorySeparatorChar);
                    string saveName = parts[parts.Length - 1];
                    saveNames.Add(saveName);
                    loadNames.Add(saveName);
                }
            }
            Redraw();
        }


        private void Redraw()
        {

            foreach (Transform child in saveContentArea.transform)
            {
                Destroy(child.gameObject);
            }
            saveButtons.Clear();
            foreach (Transform child in loadContentArea.transform)
            {
                Destroy(child.gameObject);
            }
            loadButtons.Clear();
            for (int i = 0; i < saveNames.Count; i++)
            {
                SaveButtonUI saveButton = Instantiate(saveMenuItemPrefab, saveContentArea.transform).GetComponent<SaveButtonUI>();
                saveButton.SetText(saveNames[i]);
                saveButton.SetOwner(this);
                saveButtons.Add(saveButton);
            }
            for (int i = 0; i < loadNames.Count; i++)
            {
                LoadButtonUI loadButton = Instantiate(loadMenuItemPrefab, loadContentArea.transform).GetComponent<LoadButtonUI>();
                loadButton.SetText(saveNames[i]);
                loadButton.SetOwner(this);
                loadButtons.Add(loadButton);
            }
        }


        public void SetToSavePanel()
        {
            savePanel.SetActive(true);
            loadPanel.SetActive(false);
            saveButtonImg.sprite = buttonOn;
            loadButtonImg.sprite = buttonOff;
        }


        public void SetToLoadPanel()
        {
            savePanel.SetActive(false);
            loadPanel.SetActive(true);
            saveButtonImg.sprite = buttonOff;
            loadButtonImg.sprite = buttonOn;
        }


        public void PlayButtonSound()
        {
            GameManager.Instance.UIManager.PlayButtonClick();
        }


        public void SelectAsSave(SaveButtonUI saveButton)
        {
            for (int i = 0; i < saveButtons.Count; i++)
            {
                saveButtons[i].SetSelected(saveButtons[i] == saveButton);
            }
            fileToSave = saveButton.GetText();
        }


        public void SelectAsLoad(LoadButtonUI loadButton)
        {
            for (int i = 0; i < loadButtons.Count; i++)
            {
                loadButtons[i].SetSelected(loadButtons[i] == loadButton);
            }
            fileToLoad = loadButton.GetText();
        }


        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(fileToSave))
            {
                SavedGame savedGame = new();
                savedGame.Save(fileToSave);
            }
        }


        public void DeleteSave()
        {
            if (!string.IsNullOrWhiteSpace(fileToSave))
            {
                SavedGame.DeleteSave(fileToSave);
                OnEnable();
            }
        }


        public void Load()
        {
            if (!string.IsNullOrWhiteSpace(fileToLoad))
            {
                loadingScreen.SetActive(true);
                Time.timeScale = 0.0f; // FIXME: The pause should happen when the GUI is activated
                EntityManagement.playerCharacter.Inventory.Clear();
                StartCoroutine(LoadHelper());
            }
        }


        private IEnumerator LoadHelper()
        {
            yield return new WaitForEndOfFrame();
            SavedGame savedGame = new();
            savedGame.LoadWorld(fileToLoad);
            PCData pcData = savedGame.LoadPlayer(fileToLoad, EntityManagement.playerCharacter.GetPCData());
            EntityManagement.playerCharacter.SetPCData(pcData);
            yield return new WaitForEndOfFrame();
            EntityManagement.playerCharacter.Inventory.OnEnable();
            EntityManagement.playerCharacter.Spells.OnEnable();
            InventoryManagement.SignalLoadNPCInventoryData();
            WorldManagement.SignalGameReloaded();
            loadingScreen.SetActive(false);
            Time.timeScale = 1.0f;
            InventoryManagement.SignalCloseUIs();
            GameManager.Instance.UIManager.CloseCharacterSheet();
            GameManager.Instance.UIManager.HidePauseMenu();
            EntityManagement.playerCharacter.AllowActions(true);
        }


        public void SetVisible()
        {
            Time.timeScale = 0.0f;
            EntityManagement.playerCharacter.AllowActions(false);
            Cursor.lockState = CursorLockMode.None;
            shower.SetVisible();
        }


        public void SetVisible(bool show)
        {
            if (show) Time.timeScale = 1.0f;
            else Time.timeScale = 0.0f;
            Time.timeScale = 0.0f;
            bool canMove = !(show || GameManager.Instance.UIManager.CharacterSheetVisible);
            EntityManagement.playerCharacter.AllowActions(canMove);
            if (canMove) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
            shower.SetVisible(show);
        }


        public void SetHidden()
        {
            if (!GameManager.Instance.UIManager.PauseMenuVisible)
            {
                Time.timeScale = 1.0f;
                bool canMove = !GameManager.Instance.UIManager.CharacterSheetVisible;
                EntityManagement.playerCharacter.AllowActions(canMove);
                if (canMove) Cursor.lockState = CursorLockMode.Locked;
                else Cursor.lockState = CursorLockMode.None;
            }
            shower.SetHidden();
        }


        public void Toggle()
        {
            shower.Toggle();
            bool canMove = !(shower.IsVisible || GameManager.Instance.UIManager.CharacterSheetVisible);
            EntityManagement.playerCharacter.AllowActions(canMove);
            if (canMove) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
            if (shower.IsVisible) Time.timeScale = 0.0f;
            else Time.timeScale = 1.0f;
        }


        public void AddNewSave()
        {
            SaveButtonUI saveButton = Instantiate(saveMenuItemPrefab, saveContentArea.transform).GetComponent<SaveButtonUI>();
            saveButton.SetOwner(this);
            saveButton.SetAsNewSave();
        }


        public void SolidifyNewSaveButton(SaveButtonUI newButton)
        {
            string goodName = string.Concat(newButton.GetText().Split(Path.GetInvalidFileNameChars()));
            goodName = string.Concat(goodName.Split(ForbidenChars));
            saveNames.Add(goodName);
            fileToSave = goodName;
            Save();
            OnEnable();
        }


    }


}