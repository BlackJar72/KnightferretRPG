using UnityEngine;
using System.Collections.Generic;
using System.Collections;


namespace kfutils.rpg {

    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour
    {


        [SerializeField] UIManager ui;
        public UIManager UIManager { get => ui; }

        [SerializeField] GameItemSet itemsInGame;
        public GameItemSet ItemsInGame => itemsInGame;

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
            // FIXME/TODO: This needs to be moved to a pre-play system to be loaded at start (once there is a start screen)
            WorldManagement.SetupWorldspaceRegistry(worldspaces);
            // Makeing this a true singleton, and warning with an error message if extra copies were made
            if (instacnce == null) instacnce = this;
            else if (instacnce != this)
            {
                Debug.LogError("WARNING! GameManager was placed in scenes more than once, at "
                            + instacnce.gameObject.name + " and then at " + gameObject.name + "!");
                Destroy(instacnce);
                instacnce = this;
            }
            // FIXME: This should ultimately be run at start-up, not entry into gameplay, once a start screen is added
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
            startingWorldspace.LoadAsSpawn();
            StartCoroutine(DoPostInitialLoad());
            if (ui == null) ui = GetComponent<UIManager>();
            if (ui == null) Debug.LogError("No UI Manager provided for game manager!");
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

        

    }

}