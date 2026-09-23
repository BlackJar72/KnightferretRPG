using UnityEngine;


namespace kfutils.rpg
{


    [CreateAssetMenu(menuName = "KF-RPG/Magic/Game Spell Set", fileName = "SpellSet", order = 1)]
    public class GameSpellSet : ScriptableObject
    {


        [SerializeField] Spell[] spells;


        public Spell[] Spells => spells;

#if UNITY_EDITOR
        public void Awake()
        {
            for (int i = 0; i < spells.Length; i++) Debug.Log(spells[i]);
        }
#endif

    }


}
