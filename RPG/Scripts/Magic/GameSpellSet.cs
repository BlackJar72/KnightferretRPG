using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{


    [CreateAssetMenu(menuName = "KF-RPG/Magic/Game Spell Set", fileName = "SpellSet", order = 1)]
    public class GameSpellSet : ScriptableObject
    {


        [SerializeField] Spell[] spells;

        private Dictionary<Spell, int> intDictionary;
        private Dictionary<string, Spell> spellDictionary;
        private Dictionary<Spell, string> stringDictionary;
        private Dictionary<string, int> idDictionary;

        public Spell[] Spells => spells;


        public void Awake()
        {
            intDictionary    = new();
            spellDictionary  = new();
            stringDictionary = new();
            idDictionary     = new();
            for (int i = 0; i < spells.Length; i++) {
                spells[i].SetNumberID(i);
                intDictionary.Add(spells[i], i);
                spellDictionary.Add(spells[i].ID, spells[i]);
                stringDictionary.Add(spells[i], spells[i].ID);
                idDictionary.Add(spells[i].ID, i);
#if UNITY_EDITOR
                Debug.Log(spells[i]);
#endif
            }
        }


        public int GetIntID(Spell spell) => intDictionary[spell];
        public Spell GetSpell(int id) => spells[id];


    }


}
