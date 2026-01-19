using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public class KeyedAttribute<T>
    {
        [SerializeField] string key;
        [SerializeField] T value;

        public string Key => key;
        public T Value => value;


        public void SetValue(T newValue) => value = newValue;
    }


}
