using UnityEngine;

namespace kfutils.rpg {


    [System.Serializable]
    public class EntityBaseStats
    {
        public const int MIN_SCORE = 1; // Minimum score for a player character
        public const int MAX_SCORE = 20; // Maximum score for a player character (or any human)
        public const int DEFAULT_SCORE = 10; // Average score for a human, and default for a player character

        [SerializeField] int strength = DEFAULT_SCORE;
        [SerializeField] int agility = DEFAULT_SCORE;
        [SerializeField] int vitality = DEFAULT_SCORE;
        [SerializeField] int endurance = DEFAULT_SCORE;
        [SerializeField] int intelligence = DEFAULT_SCORE;
        [SerializeField] int charisma = DEFAULT_SCORE;
        [SerializeField] int spirit = DEFAULT_SCORE;

        public int Strength { get => strength; set => strength = value; }
        public int Agility { get => agility; set => agility = value; }
        public int Vitality { get => vitality; set => vitality = value; }
        public int Endurance { get => endurance; set => endurance = value; }    
        public int Intelligence { get => intelligence; set => intelligence = value; }
        public int Charisma { get => charisma; set => charisma = value; }
        public int Spirit { get => spirit; set => spirit = value; } 

        public void GenRandomHumanStats() {
            strength = RollStat();
            agility = RollStat();
            vitality = RollStat();
            endurance = RollStat();
            intelligence = RollStat();
            charisma = RollStat();
            spirit = RollStat();
        }


        private int RollStat() {
            return Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7) + 2;
            // return 20;
        }


        public EntityBaseStats Copy() {
            EntityBaseStats copy = new();
            copy.strength = strength;
            copy.agility = agility;
            copy.vitality = vitality;
            copy.endurance = endurance;
            copy.intelligence = intelligence;
            copy.charisma = charisma;
            copy.spirit = spirit;
            return copy;
        }


        public void CopyInto(EntityBaseStats other) {
            strength = other.strength;
            agility = other.agility;
            vitality = other.vitality;
            endurance = other.endurance;
            intelligence = other.intelligence;
            charisma = other.charisma;
            spirit = other.spirit;
        }

    }

}