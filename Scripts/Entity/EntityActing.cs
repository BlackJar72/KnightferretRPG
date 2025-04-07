using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, IActor
    {



        /*// Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            
        }*/

        // Update is called once per frame
        /*protected override void Update()
        {
            
        }*/


        public void EquiptItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public void UnequiptItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public void UnequiptItem(EEquiptSlot slot)
        {
            throw new System.NotImplementedException();
        }

        
    }


}
