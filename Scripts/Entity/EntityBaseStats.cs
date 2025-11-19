using UnityEngine;

namespace kfutils.rpg {


    [System.Serializable]
    public enum EBaseStats
    {
        Strength = 0,
        Agility = 1,
        Vitality = 2,
        Endurance = 3,
        Intelligence = 4,
        Willpower = 5,
        Charisma = 6,
        Spirit = 7 
    }


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
        [SerializeField] int willpower = DEFAULT_SCORE;
        [SerializeField] int charisma = DEFAULT_SCORE;
        [SerializeField] int spirit = DEFAULT_SCORE;

        public int Strength { get => strength; set => strength = value; }
        public int Agility { get => agility; set => agility = value; }
        public int Vitality { get => vitality; set => vitality = value; }
        public int Endurance { get => endurance; set => endurance = value; }    
        public int Intelligence { get => intelligence; set => intelligence = value; }  
        public int Willpower { get => willpower; set => willpower = value; }
        public int Charisma { get => charisma; set => charisma = value; }
        public int Spirit { get => spirit; set => spirit = value; }

        /// <summary>
        /// Not intdended to be commonly used, but in some situations this is preferable. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public int this[EBaseStats index]
        {
            get
            {
                return index switch
                {
                    EBaseStats.Strength => strength,
                    EBaseStats.Agility => agility,
                    EBaseStats.Vitality => vitality,
                    EBaseStats.Endurance => endurance,
                    EBaseStats.Intelligence => intelligence,
                    EBaseStats.Willpower => willpower,
                    EBaseStats.Charisma => charisma,
                    EBaseStats.Spirit => spirit,
                    _ => throw new System.IndexOutOfRangeException("Non-existent index passed to EntityBaseStats indexer."),
                };
            }
            set // Set is needed for character creation, but should not normally be used
            {
                switch(index) {
                   case EBaseStats.Strength: strength = value; return;
                   case EBaseStats.Agility: agility = value; return;
                   case EBaseStats.Vitality: vitality = value; return; 
                   case EBaseStats.Endurance: endurance = value; return;
                   case EBaseStats.Intelligence: intelligence = value; return;
                   case EBaseStats.Willpower: willpower = value; return;  
                   case EBaseStats.Charisma: charisma = value; return;
                   case EBaseStats.Spirit: spirit = value; return; 
                   default: throw new System.IndexOutOfRangeException("Non-existent index passed to EntityBaseStats indexer.");
                }
            }        
        }


        public void GenRandomHumanStats() {
            strength = RollStat();
            agility = RollStat();
            vitality = RollStat();
            endurance = RollStat();
            intelligence = RollStat();
            willpower = RollStat();
            charisma = RollStat();
            spirit = RollStat();
        }


        private int RollStat() {
            return Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(0, 3); 
        }


        public EntityBaseStats Copy() {
            EntityBaseStats copy = new();
            copy.strength = strength;
            copy.agility = agility;
            copy.vitality = vitality;
            copy.endurance = endurance;
            copy.intelligence = intelligence;
            copy.willpower = willpower;
            copy.charisma = charisma;
            copy.spirit = spirit;
            return copy;
        }


        public void CopyInto(EntityBaseStats other)
        {
            strength = other.strength;
            agility = other.agility;
            vitality = other.vitality;
            endurance = other.endurance;
            intelligence = other.intelligence;
            willpower = other.willpower;
            charisma = other.charisma;
            spirit = other.spirit;
        }
        


    }

}