using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using kfutils.rpg.ui;


namespace kfutils.rpg {

    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {


        [SerializeField] UIManager ui;
        public UIManager UI => ui;

        [SerializeField] GameItemSet itemsInGame;
        public GameItemSet ItemsInGame => itemsInGame;

        [SerializeField] GameObject playerCharacterPrefab;
        [SerializeField] Vector3 pcSpawnPosition;
        [SerializeField] Quaternion pcSpawnRotation;
        [SerializeField] PCTalking playerCharacter;
        [SerializeField] GameObject weather;

        [SerializeField] StartMenu startLogic;
        public StartMenu StartLogic => startLogic;

        private static GameManager instacnce;
        public static GameManager Instance { get => instacnce; }

        [SerializeField] Worldspace startingWorldspace;
        // FIXME/TODO: This needs to be moved to a pre-play system to be loaded at start (once there is a start screen)
        [SerializeField] Worldspace[] worldspaces;

        public delegate bool SpecialMethod();
        public List<SpecialMethod> specialUpdates = new();

        [SerializeField] GameObject loadingScreen;
        public GameObject LoadingScreen => loadingScreen;


        void Awake()
        {
            #if UNITY_EDITOR
            itemsInGame.Awake();
            #endif
            instacnce = this;
            ItemPrototype[] itemPrototypes = itemsInGame.Items;
            for (int i = 0; i < itemPrototypes.Length; i++)
            {
                ItemManagement.AddItemPrototype(itemPrototypes[i]);
            }
            EntityManagement.Initialize();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ui = GetComponent<UIManager>();
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


        public void EnterPlayMode()
        {
            InitializeNewCharacter();
            WorldManagement.SetupWorldspaceRegistry(worldspaces);
            NewGame();
            ui.ShowInGameUI();
            weather.SetActive(true);
            playerCharacter.gameObject.SetActive(true);
            startingWorldspace.LoadAsSpawn();
            StartCoroutine(DoPostInitialLoad());            
        }


        public void EnterStartMenu()
        {
            ui.ShowStartUI();
            startLogic.EnterStartMenu();
            weather.SetActive(false);
            Destroy(playerCharacter.gameObject);      
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


        public void SetPCActive(bool value)
        {
            playerCharacter.gameObject.SetActive(value);
        }


        public void InitializeNewCharacter()
        {
            GameObject pcgo = Instantiate(playerCharacterPrefab, pcSpawnPosition, pcSpawnRotation, transform.root);
            playerCharacter = pcgo.GetComponent<PCTalking>();
            ui.InitUI(playerCharacter);
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
            yield return new WaitForEndOfFrame();
            UI.ShowLoadingScreen();
            SavedGame savedGame = new();
            savedGame.LoadWorld(saveToLoad);
            PCData pcData = savedGame.LoadPlayer(saveToLoad, EntityManagement.playerCharacter.GetPCData());
            EntityManagement.playerCharacter.SetPCData(pcData);
            yield return null;
            yield return new WaitForEndOfFrame();
            EntityManagement.playerCharacter.Inventory.OnEnable();
            EntityManagement.playerCharacter.Spells.OnEnable();
            InventoryManagement.SignalLoadNPCInventoryData();
            WorldManagement.SignalGameReloaded();
            UI.HideLoadingScreen();
            Time.timeScale = 1.0f;
            InventoryManagement.SignalCloseUIs();
            GameManager.Instance.UI.CloseCharacterSheet();
            GameManager.Instance.UI.HidePauseMenu();
            EntityManagement.playerCharacter.AllowActions(true);
        }

        

    }

}