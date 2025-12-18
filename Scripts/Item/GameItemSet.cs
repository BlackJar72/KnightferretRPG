using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace kfutils.rpg
{


    [CreateAssetMenu(menuName = "KF-RPG/Items/Game Item Set", fileName = "ItemSet", order = 0)]
    public class GameItemSet : ScriptableObject, IEnumerable<ItemPrototype> 
    {


        [SerializeField] ItemPrototype[] items;

        //public ItemPrototype[] Items => items;
        public ItemPrototype this[int index] => items[index];
        public int Length => items.Length;


        public IEnumerator<ItemPrototype> GetEnumerator()
        {
            int i = 0;
            while(i < items.Length) yield return items[i++];
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


#if UNITY_EDITOR
        // Unity seems loose this data, but using it on start seems to prevent that.
        public void Awake()
        {
            System.Text.StringBuilder dummy = new("");
            for (int i = 0; i < items.Length; i++) dummy.Append(items[i]);
        }
#endif

    }


}
