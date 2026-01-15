using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/World/Game World Effects Set", fileName = "EffectsList", order = 200)]
    public class GameEffecttList : ScriptableObject, IEnumerable<EffectPrototype>
    {
        [SerializeField] EffectPrototype[] worldEffects;

        public EffectPrototype this[int index] => worldEffects[index];
        public int Length => worldEffects.Length;


        public IEnumerator<EffectPrototype> GetEnumerator()
        {
            int i = 0;
            while(i < worldEffects.Length) yield return worldEffects[i++];
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


#if UNITY_EDITOR
        // Unity seems loose this data, but using it on start seems to prevent that.
        public void Awake()
        {
            StringBuilder dummy = new("");
            for (int i = 0; i < worldEffects.Length; i++) dummy.Append(worldEffects[i]);
        }
#endif

    }

}
