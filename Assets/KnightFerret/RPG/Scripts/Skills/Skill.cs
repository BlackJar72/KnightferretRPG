using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class Skill
    {
        public const int MAX_SCORE = 20;
        
        [SerializeField] EBaseStats baseStat;
        [Range(0, MAX_SCORE)]
        [SerializeField] int adds = 0;
        [ES3Serializable] int total;

        private EntityActing owner; // Should this belong to EntityTalking? Attributes?

        public int Adds => adds;
        public int Total => total;

        public static implicit operator int(Skill skill) => skill.total;
        public static bool operator ==(Skill a, Skill b) => a.total == b.total; 
        public static bool operator !=(Skill a, Skill b) => a.total != b.total; 
        public static bool operator >(Skill a, Skill b) => a.total > b.total;
        public static bool operator <(Skill a, Skill b) => a.total < b.total;
        public static int operator +(Skill a, Skill b) => a.total + b.total;
        public static int operator -(Skill a, Skill b) => a.total - b.total;
        public static int operator *(Skill a, Skill b) => a.total * b.total;
        public static int operator /(Skill a, Skill b) => a.total / b.total;
        public static Skill operator ++(Skill skill) { skill.Increase(); return skill; }
        public static Skill operator --(Skill skill) { skill.Decrease(); return skill; }
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();


        public void Init(EntityActing owner)
        {
            this.owner = owner;
            SetTotal();
        }


        /// <summary>
        /// Fix the skill to its correct value. This *MUST* be called after altering the skill 
        /// *OR* its underlying stat.
        /// </summary>
        public void SetTotal()
        {
            adds = Mathf.Clamp(adds, 0, MAX_SCORE);
            total = owner.attributes.baseStats[baseStat] + adds;
        }


        public int Increase()
        {
            adds++;
            SetTotal();
            return adds;
        }


        public int Increase(int amount)
        {
            adds += amount;
            SetTotal();
            return adds;
        }


        public int Decrease()
        {
            adds--;
            SetTotal();
            return adds;
        }


        public int Decrease(int amount)
        {
            adds -= amount;
            SetTotal();
            return adds;
        }


        public void Set(int value)
        {
            adds = value;
            SetTotal();
        }



    }

}
