using UnityEngine;
using kfutils.rpg.ui;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;



namespace kfutils.rpg {

    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour {

        [SerializeField] UIManager ui;
        public UIManager UIManager { get => ui; }

        private static GameManager instacnce;
        public static GameManager Instance { get => instacnce; }

        [SerializeField] Worldspace startingWorldspace;

        public delegate void SpecialMethod();
        public List<SpecialMethod> specialUpdates = new();


        void Awake() {
            // Makeing this a true singleton, and warning with an error message if extra copies were made
            if(instacnce == null) instacnce = this;
            else if(instacnce != this) {
                Debug.LogError("WARNING! GameManager was placed in scenes more than once, at " 
                            + instacnce.gameObject.name + " and then at " + gameObject.name + "!");
                Destroy(instacnce);
                instacnce = this;
            }
            EntityManagement.Initialize();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            startingWorldspace.LoadAsSpawn();
            if(ui == null) ui = GetComponent<UIManager>(); 
            if(ui == null) Debug.LogError("No UI Manager provided for game manager!"); 
        }


        // Update is called once per frame
        void Update()
        {
            EntityManagement.Update();
            // Other game management stuff will go here
            InventoryManagement.DoRedraws();
            for(int i = specialUpdates.Count - 1; i > -1; i--) specialUpdates[i]();            
        }


    }

    

}
