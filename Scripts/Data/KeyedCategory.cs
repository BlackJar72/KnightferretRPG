using UnityEngine;
using System.Collections.Generic;


namespace kfutils.rpg {


    [System.Serializable]
    public class KeyedCategory<T>
    {
        [SerializeField] List<KeyedAttribute<T>> attributes;

        private Dictionary<string, T> dictionary;

        public T this[string key] => dictionary[key];

        public T this[int index] => attributes[index].Value;


        /// <summary>
        /// This MUST be called before accessing the dictionary!
        /// 
        /// This adds all items from the list to the dictionary, using the items key as the key and the items 
        /// value as the value.  Items with duplicate keys will be removed after the first occurance.
        /// 
        /// Ideally this will be called in Awake() and with serialization callbacks. 
        /// </summary>
        public void SyncData()
        {
            dictionary.Clear();
            for(int i = attributes.Count; i > -1; i--)
            {
                if(!dictionary.ContainsKey(attributes[i].Key)) dictionary.Add(attributes[i].Key, attributes[i].Value);
                else 
                {
                    Debug.LogWarning("Key \"" + attributes[i].Key + "\" appeared twice; second occurance was at index " 
                                    + i + " with value of " + attributes[i].Value);
                    attributes.RemoveAt(i);
                }
            }
        }


    }


}

