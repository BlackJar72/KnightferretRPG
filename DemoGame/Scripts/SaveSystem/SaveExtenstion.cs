using UnityEngine;
using kfutils.rpg;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DistantLands.Cozy;
using DistantLands.Cozy.Data;
using System;
using static rpg.verslika.HumanBody;
using UnityEngine.SceneManagement;


namespace rpg.verslika {


    public class SaveExtenstion : ASaveExtention 
    {
        private static HumanBodyData loadedBody;

        public override void Save(string fileName)
        {
            if(EntityManagement.playerCharacter.TryGetComponent<HumanBody>(out HumanBody pcBody))
            {
                loadedBody = new(pcBody);
                ES3.Save("PCBody", loadedBody, fileName);
                // string cozyData = CozyWeather.instance.GetModule<CozySaveLoadModule>().SaveToExternalJSON();
                // string cozySave = fileName + ".cw3";
                // ES3.SaveRaw(cozyData, cozySave); 
            }
        }


        public override void LoadWorld(string fileName)
        {
            string cozySave = fileName + ".cw3";
            if(ES3.FileExists(cozySave)) {
                string cozyData = ES3.LoadRawString(cozySave);
                // CozyWeather.instance.GetModule<CozySaveLoadModule>().LoadFromExternalJSON(cozyData);
            }
        }


        public override void LoadPlayer(string fileName)
        {
            try {
                if(ES3.KeyExists("PCBody", fileName)) {
                    loadedBody = ES3.Load<HumanBodyData>("PCBody", fileName);
                    if(EntityManagement.playerCharacter.TryGetComponent<HumanBody>(out HumanBody pcBody)) {
                        pcBody.CopyInto(loadedBody);
                        pcBody.RestoreOnLoad();
                        pcBody.RestoreArmsOnLoad();
                    }
                }
            } 
            catch (Exception e)
            {
                Debug.LogException(e);
                Application.Quit();
            }
        }  



        public override void DeleteSave(string fileName)
        {
            string cozySave = fileName + ".cw3";
            ES3.DeleteFile(cozySave);
        }


    }

}
