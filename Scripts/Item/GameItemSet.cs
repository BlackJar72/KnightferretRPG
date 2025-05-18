using UnityEngine;


namespace kfutils.rpg
{


    [CreateAssetMenu(menuName = "KF-RPG/Items/Game Item Set", fileName = "ItemSet", order = 0)]
    public class GameItemSet : ScriptableObject {


        [SerializeField] ItemPrototype[] items;


        public ItemPrototype[] Items => items;



    }


}