using Animancer.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {


    [CreateAssetMenu(menuName = "KF-RPG/World/World Space", fileName = "WorldSpace", order = 10)]
    public class Worldspace : ScriptableObject {

        [SerializeField] string scenePath;
        [SerializeField] float seaLevel;


        public string ScenePath => scenePath;
        public float SeaLevel => seaLevel;


        /// <summary>
        /// Loads the world space, unloading the previous world space.
        /// This pauses time while loading syncronously (not asynchronously) so that the scene 
        /// can definitely be fully loaded before resuming gameplay.
        /// </summary>
        /// <param name="old">A previously loaded worldspace (whose scene must be unloaded)</param>
        public void Load(Worldspace old = null) {
            Time.timeScale = 0.0f;
            if(old != null) SceneManager.UnloadSceneAsync(old.scenePath);
            SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            WorldManagement.SetWorldspace(this);
            Time.timeScale = 1.0f;
        }
    }

}
