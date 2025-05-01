using kfutils.rpg;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {

    public static class WorldManagement {

        private static Worldspace worldspace;


        public static Worldspace CurWorldspace => worldspace;
        public static float SeaLevel => worldspace == null ? float.MinValue : worldspace.SeaLevel;


        public static void SetWorldspace(Worldspace world) {
            worldspace = world;
        }


        public static void SpecialUpdate() {}


        static GameManager.SpecialMethod update = SpecialUpdate;


        public static void TransferPC(PCMoving pc, Worldspace next, TransformData destination) {
            Time.timeScale = 0.0f;
            if(CurWorldspace != null) SceneManager.UnloadSceneAsync(CurWorldspace.ScenePath);
            SceneManager.LoadScene(next.ScenePath, LoadSceneMode.Additive);
            SetWorldspace(next);               
            pc.Teleport(destination);
            Time.timeScale = 1.0f;
        }


    }


}