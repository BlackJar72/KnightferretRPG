using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/World/World Effect Prototype", fileName = "EffectPrototype", order = 210)]
    public class EffectPrototype : ScriptableObject, IHaveStringID
    {
        [SerializeField] string id;
        [SerializeField] WorldEffect effectPrototype;

        public string ID => id;
        public WorldEffect EffectProto => effectPrototype;
        public GameObject ObjProto => effectPrototype.gameObject;
    }

}
