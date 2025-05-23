using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, IActor
    {
        public AnimancerLayer ActionLayer => throw new System.NotImplementedException();

        public AnimancerState ActionState => throw new System.NotImplementedException();



        /*// Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            
        }*/

        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/



        public void EquiptItem(ItemStack item) {
            throw new System.NotImplementedException();
        }


        public virtual void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
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
