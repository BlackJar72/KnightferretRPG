using Animancer.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kfutils.rpg {


    [CreateAssetMenu(menuName = "KF-RPG/World/World Space", fileName = "WorldSpace", order = 10)]
    public class Worldspace : ScriptableObject {

        [Tooltip("The location of the scene file.")]
        [SerializeField] string scenePath;
        [SerializeField] bool multiChunk;
        [Tooltip("The Y coordinate below which is considered underwater; used for visual effect and switching movement to swim mode.")]
        [SerializeField] float seaLevel;
        [SerializeField] TransformData defaultStartLocation;
        [Tooltip("The size of the chunk along the X and Z dimension (which should always be the same); should usually be a power of 2.")]
        [SerializeField] int chunkSize = 128;
        [Tooltip("The transform position of the first chunk, chunk[0,0]")]
        [SerializeField] Vector2 chunkOffset;


        public string ScenePath => scenePath;
        public bool MultiChunk => multiChunk;
        public float SeaLevel => seaLevel;
        public TransformData DefaultStart => defaultStartLocation;
        public int ChunkSize => chunkSize;
        public Vector2 ChunkOffset => chunkOffset;
        public float ChunkOffsetX => chunkOffset.x;
        public float ChunkOffsetZ => chunkOffset.y;


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


        /// <summary>
        /// Loads the world space, unloading the previous world space.
        /// This pauses time while loading syncronously (not asynchronously) so that the scene 
        /// can definitely be fully loaded before resuming gameplay.
        /// </summary>
        /// <param name="old">A previously loaded worldspace (whose scene must be unloaded)</param>
        public void LoadAsSpawn(Worldspace old = null) {
            Time.timeScale = 0.0f;
            if(old != null) SceneManager.UnloadSceneAsync(old.scenePath);
            SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
            WorldManagement.SetWorldspace(this);
            EntityManagement.playerCharacter.Teleport(defaultStartLocation);
            Time.timeScale = 1.0f;
        }


    }

}
