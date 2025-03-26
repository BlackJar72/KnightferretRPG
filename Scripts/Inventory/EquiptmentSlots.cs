using UnityEngine;
using kfutils.rpg.ui;


namespace kfutils.rpg {

    public class EquiptmentSlots : MonoBehaviour, IInventory {

        [SerializeField] ItemStack[] slots = new ItemStack[12];



    }

}