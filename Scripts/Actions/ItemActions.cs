using UnityEngine;
using Animancer;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Item Action Set", fileName = "ItemActions", order = 30)]
    public class ItemActions : ScriptableObject
    {

        [Tooltip("Primary action (attack for weapons)")][SerializeField] AbstractAction primary;
        [Tooltip("Secondary action (block for melee weapons)")][SerializeField] AbstractAction secondary;
        [Tooltip("Special MovementSet when equiped if any (null in none)")][SerializeField] MovementSet movement;

        [Tooltip("Sound to play for when primary action in played")][SerializeField] Sound primarySound;
        [Tooltip("Sound to play for when scondary action in played")][SerializeField] Sound secondarySound;

        public AbstractAction Primary => primary;
        public AbstractAction Secondary => secondary;
        public MovementSet Movement => movement;

        public Sound PrimarySound => primarySound;
        public Sound SecondarySound => secondarySound;


        public ItemActions Duplicate()
        {
            ItemActions output = Instantiate(this);
            output.primary = primary.Duplicate();
            output.secondary = secondary.Duplicate();
            return output;
        }




    }
    

}
