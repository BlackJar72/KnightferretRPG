using System;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, IActor
    {

        [SerializeField] CharacterInventory inventory;
        [SerializeField] Spellbook spellbook;
        [SerializeField] CharacterEquipt itemLocations;

        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;

        public AnimancerLayer ActionLayer => actionLayer;
        public AnimancerState ActionState => actionState;



        /*// Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            
        }*/

        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/


        protected override void Start()
        {
            base.Start();
            // Line only to 
            if (alive)
            {
                SetDestination(EntityManagement.playerCharacter.transform.position); // REMOVE ME
                SetMoveType(MoveType.walk); // REMOVE ME
            }
        }


        public void EquiptItem(ItemStack item) {
            throw new System.NotImplementedException();
        }


        public virtual void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
        }

        public void PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {
            throw new NotImplementedException();
        }

        public void PlayAction(AvatarMask mask, ITransition animation, Action onEnd, float time = 0, float delay = 1.0f)
        {
            throw new NotImplementedException();
        }

        public void PreSaveEquipt()
        {
            // TODO: This
            Debug.Log(ID + " => PreSaveEquipt()");
        }

        public void RemoveEquiptAnimation() {
            throw new System.NotImplementedException();
        }


        public void UnequiptItem(ItemStack item) {
            throw new System.NotImplementedException();
        }


        public void UnequiptItem(EEquiptSlot slot) {
            throw new System.NotImplementedException();
        }

        
    }


}
