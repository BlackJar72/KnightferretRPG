using UnityEngine;


namespace kfutils.rpg.ui
{


    public class LoadHandler 
    {
        private static string fileToLoad = null;
        private static AsyncOperation loadOpperation;

        private static float completion = 0.0f;
        private static bool completed = false;

        public static float Completion => completion;
        public static bool Commpleted => completed;


        public static void LoadSave(string saveFile)
        {
            fileToLoad = saveFile;

        }


    }


}