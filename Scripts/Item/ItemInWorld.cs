using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {
    
    public class ItemInWorld : MonoBehaviour, IInteractable {
        
        [SerializeField] ItemPrototype prototype;


        public void Use(GameObject from)
        {
            Inventory toInv = from.GetComponent<Inventory>();
            if(toInv != null) {
                toInv.AddToFirstEmptySlot(new ItemStack(prototype, 1, -1));
                GameObject.Destroy(gameObject);
            }
        }



    }


}
