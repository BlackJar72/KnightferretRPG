using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;


namespace kfutils.rpg {

    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {

        // FIXME?  This should probably be formatted like other classes in the project, 
        //         not in this weird, idiosyncratic way (even if it makes its on kind of sense).
        [SerializeField] UIManager ui;
        public UIManager UI => ui;

        [SerializeField] GameItemSet itemsInGame;
        public GameItemSet ItemsInGame => itemsInGame;

        [SerializeField] GameEffecttList worldEffectsInGame;
        public GameEffecttList EffectsInGame => worldEffectsInGame;

        private static GameManager instance;
        public static GameManager Instance { get => instance; }

        [SerializeField] Worldspace startingWorldspace;
        public Worldspace StartingWorldspace => startingWorldspace; 
        [SerializeField] Worldspace[] worldspaces;
        [SerializeField] PCTalking playerCharacter;
        [SerializeField] InitialPCData initialPCData;

        [SerializeField] GameObject weather;

        public delegate bool SpecialMethod();
        public List<SpecialMethod> specialUpdates = new();

        [SerializeField] GameObject loadingScreen;
        public GameObject LoadingScreen => loadingScreen;

        [SerializeField] ASaveExtention saveExtention;
        public ASaveExtention SaveExtention => saveExtention;


        private bool inGame = false;
        public bool InGame => inGame;


        void Awake()
        {
            instance = this;
            #if UNITY_EDITOR
            itemsInGame.Awake();
            #endif
            WorldManagement.SetupWorldspaceRegistry(worldspaces);
            for (int i = 0; i < itemsInGame.Length; i++)
            {
                ItemManagement.AddItemPrototype(itemsInGame[i]);
            }
            for (int i = 0; i < worldEffectsInGame.Length; i++)
            {
                ObjectManagement.AddEffectPrototype(worldEffectsInGame[i]);
            }
            EntityManagement.Initialize();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ui = GetComponent<UIManager>();
            EntityManagement.playerCharacter = playerCharacter;
        }


        // Update is called once per frame
        void Update()
        {
            EntityManagement.Update();
            // Other game management stuff will go here
            InventoryManagement.DoRedraws();
            for (int i = specialUpdates.Count - 1; i > -1; i--)
            {
                if (specialUpdates[i]()) specialUpdates.RemoveAt(i);
            }
        }


        public void LoadStartingWorld()
        {            
            startingWorldspace.LoadAsSpawn();
            StartCoroutine(DoPostInitialLoad());
        }


        public void EnterStartMenu()
        {
            inGame = false;
            WorldManagement.UnloadWorld();
            playerCharacter.gameObject.SetActive(false);
            weather.SetActive(false);
            ui.EnterStartMenu();
        }


        public void EnterPlayMode()
        {
            inGame = true;
            playerCharacter.gameObject.SetActive(true);
            weather.SetActive(true); 
            ui.EnterPlayMode();
            // Test
            SetAmbient(new(0.10f, 0.10f, 0.15f, 1.0f));
        }


        public static void ReturnFromSpecialCanvas()
        {
            instance.ui.ReturnFromSpecialCanvas(Instance.inGame);
        }


        public void EnterPlayModeStaged()
        {
            playerCharacter.gameObject.SetActive(true);
            StartCoroutine(EnterPlayModeNext());
        }


        private IEnumerator EnterPlayModeNext()
        {
            yield return null;
            weather.SetActive(true); 
            ui.EnterPlayMode();
        }


        private void SetAmbient(Color color)
        {
            Shader.SetGlobalColor("_Ambient", color);
        }


        /// <summary>
        /// Initialize static data tables before the game starts.  This can be called before 
        /// the the GameManager instance is instatiated, and often should be. 
        /// </summary>
        public static void NewGame()
        {
            EntityManagement.NewGame();
            InventoryManagement.NewGame();
            ItemManagement.NewGame();
            ObjectManagement.NewGame();
            WorldManagement.NewGame();
        }


        public void InitializeNewPC()
        {
            playerCharacter.ResetCharacter();
            playerCharacter.InitializeNewPC(initialPCData);
        }


        public void CustomizeNewPC(string name, EntityBaseStats stats)
        {
            playerCharacter.SetName(name);
            playerCharacter.attributes.baseStats.CopyInto(stats);
            playerCharacter.attributes.DeriveAttributesForHuman(playerCharacter.health, 
                                                                playerCharacter.stamina, 
                                                                playerCharacter.mana);
            playerCharacter.NewCharacterInit();
        }


        public static void DoPostLoadForOther()
        {            
            Instance.StartCoroutine(DoPostInitialLoad());
        }


        private static IEnumerator DoPostInitialLoad()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            WorldManagement.SignalPostLoad();
        }


        public void ConitnueLoading(string saveToLoad)
        {
            NewGame();
            EnterPlayMode();
            LoadingScreen.SetActive(true);            
            StartCoroutine(ConitnueLoadHelper(saveToLoad));
        }


        public void CloseStartScreen()
        {
            EnterPlayMode();
        }


        private IEnumerator ConitnueLoadHelper(string saveToLoad)
        {
            Time.timeScale = 0.0f;
            yield return null;
            UI.ShowLoadingScreen();
            SavedGame savedGame = new();
            savedGame.LoadWorld(saveToLoad);
            PCData pcData = savedGame.LoadPlayer(saveToLoad, EntityManagement.playerCharacter.GetPCData());
            EntityManagement.playerCharacter.SetPCData(pcData);
            yield return null;
            EntityManagement.playerCharacter.Inventory.OnEnable();
            EntityManagement.playerCharacter.Spells.OnEnable();
            InventoryManagement.SignalLoadNPCInventoryData();
            WorldManagement.SignalGameReloaded();
            UI.HideLoadingScreen();
            Time.timeScale = 1.0f;
            InventoryManagement.SignalCloseUIs();
            UI.CloseCharacterSheet();
            Instance.UI.HidePauseMenu();
            EntityManagement.playerCharacter.AllowActions(true);
        }


        public void LoadForSave(Worldspace world, Worldspace old = null)
        {
            StartCoroutine(DoLoadForSave(world, old));
        }


        private IEnumerator DoLoadForSave(Worldspace world, Worldspace old = null)
        {
            float timeScale = Time.timeScale;
            Time.timeScale = 0.0f;
            if (old != null) {
                AsyncOperation unloading = SceneManager.UnloadSceneAsync(old.ScenePath);
                while(!unloading.isDone) yield return null;
                yield return null;
            }
            ObjectManagement.FinishLoadData();
            SceneManager.LoadScene(world.ScenePath, LoadSceneMode.Additive);
            WorldManagement.SetWorldspace(world);
            StartCoroutine(world.HelpSetupWorldspace());
            Time.timeScale = timeScale;
        }


        public void LoadAsSpawn(Worldspace world, Worldspace old = null)
        {
            StartCoroutine(DoLoadAsSpawn(world, old));
        }


        private IEnumerator DoLoadAsSpawn(Worldspace world, Worldspace old = null)
        {
            Time.timeScale = 0.0f;
            if (old != null) {
                AsyncOperation unloading = SceneManager.UnloadSceneAsync(old.ScenePath);
                while(!unloading.isDone) yield return null;
                yield return null;
            }
            ObjectManagement.NewGame();
            SceneManager.LoadScene(world.ScenePath, LoadSceneMode.Additive);
            WorldManagement.SetWorldspace(world);
            StartCoroutine(world.SpawnPlayerOnLoad());
        }



        

    }

}