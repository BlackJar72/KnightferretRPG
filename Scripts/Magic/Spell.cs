using UnityEngine;


namespace kfutils.rpg {
    
    /// <summary>
    /// FIXME???  Not sure I should do it this way, it jsut put the relevant field data here -- that might actually be better.
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/Magic/Spell Prototype", fileName = "Spell", order = 10)]
    public class Spell : ScriptableObject {
        
        // Identifying Data
        [SerializeField] string id;
        [SerializeField] string spellName;
        [SerializeField] Sprite icon;
        [SerializeField] string description;

        //Technical Data
        


        // Accessor Properties
        public string ID { get => id; }
        public string Name { get => spellName; }
        public Sprite Icon {get => icon; } 
        public string Description { get => description; }


        

    }

}
