using UnityEngine;
using kfutils.rpg.ui;
using UnityEngine.InputSystem;
using System;



namespace kfutils.rpg {

    [RequireComponent(typeof(UIManager))]
    public class GameManager : MonoBehaviour {

        [SerializeField] UIManager ui;
        public UIManager UIManager { get => ui; }


        void Awake()
        {
            EntityManagement.Initialize();
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(ui == null) ui = GetComponent<UIManager>();    
        }


        // Update is called once per frame
        void Update()
        {
            EntityManagement.Update();
            // Other game management stuff will go here
            InventoryManager.DoRedraws();
            
        }


    }

    

}
