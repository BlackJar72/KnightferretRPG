using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace kfutils.rpg
{


    public class SaveLoadUI : MonoBehaviour
    {


        [SerializeField] GameObject savePanel;
        [SerializeField] GameObject loadPanel;


        private string[] files;
        private List<string> saveNames = new();


        void OnEnable()
        {
            // FIXME: Some test code, to be removed (mostly?)
            saveNames.Clear();
            string folder = Application.persistentDataPath + Path.DirectorySeparatorChar + SavedGame.saveSubdir;
            Debug.Log(folder);
            files = Directory.GetFiles(folder);
            foreach (string filename in files)
            {
                Debug.Log(filename);
                if (filename.EndsWith(SavedGame.saveFileExtension))
                {
                    string[] parts = System.IO.Path.GetFileNameWithoutExtension(filename).Split(Path.DirectorySeparatorChar);
                    string saveName = parts[parts.Length - 1];
                    saveNames.Add(saveName);
                    Debug.Log(saveName);
                }
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


    }


}