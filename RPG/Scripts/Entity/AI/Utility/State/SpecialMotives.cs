using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// A class to repressent special motive attached to special AI states, notably for 
    /// work states where the NPC might move between various work related tasks. 
    /// </summary>
    [CreateAssetMenu(menuName = "KF-RPG/AI/Utility/Special Motives", fileName = "SpecialMotives", order = 100)]
    public class SpecialMotives : ScriptableObject, IHaveStringID 
    {
        [System.Serializable]
        public class Motive : Need
        {
            [SerializeField] string name; // A label so there is some way to keep track of motives
            public string Name => name;

            public Motive(string name, float decayRate, float importance, float driveOrigin = 1.2F) : base(decayRate, importance, driveOrigin)
            {
                this.name = name;
            }

            public Motive(Motive other) : base(other)
            {
                name = other.name;
            }
        }


        [SerializeField] string id;
        [SerializeField] Motive[] motives;

        public Motive[] Motives => motives;
        public string ID => id;


        /// <summary>
        /// Create a run-time copy for use by a specific NPC (or technically, AIState instance).
        /// </summary>
        /// <returns></returns>
        public SpecialMotives Duplicate()
        {
            SpecialMotives result = new SpecialMotives();
            result.id = id;
            result.motives = new Motive[motives.Length];
            for (int i = 0; i < result.motives.Length; i++) result.motives[i] = new Motive(motives[i]);
            return result;
        }


        public void UpdateMotives()
        {
            for (int i = 0; i < motives.Length; i++) motives[i].Decay();
        }


        public void AddToMotive(int motive, float amount)
        {
            motives[motive].Add(amount);
        }


        public void AlterMotiveGradual(int motive, float timeForEffect, float amount)
        {
            motives[motive].Add((amount / timeForEffect) * Time.deltaTime);
        }


        public void AlterMotiveInstant(int motive, float amount)
        {
            motives[motive].Add(amount);
        }




    }


}
