using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace kfutils.rpg
{


    public class SaveLoadUI : MonoBehaviour
    {

        [SerializeField] GameObject saveMenuItemPrefab;
        [SerializeField] GameObject loadMenuItemPrefab;

        [SerializeField] GameObject savePanel;
        [SerializeField] GameObject saveContentArea;
        [SerializeField] GameObject loadPanel;
        [SerializeField] GameObject loadContentArea;


        private string[] files;
        private List<string> saveNames = new();
        private string fileToSave = null;
        private string fileToLoad = null;

        private List<SaveButtonUI> saveButtons = new();
        private List<LoadButtonUI> loadButtons = new();


        void OnEnable()
        {
            fileToLoad = fileToSave = null; // FIXME/TODO: This should default to the last save used (or a new save if first in a new game)
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
        }


        public void SetToLoadPanel()
        {
            savePanel.SetActive(false);
            loadPanel.SetActive(true);
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


    }


}