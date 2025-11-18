using UnityEngine;
using TMPro;

namespace kfutils.rpg {


    public class StatCreationUI : MonoBehaviour
    {
        [SerializeField] int startingAdditions;
        [SerializeField] StatAdjusterUI[] statAdjusters;
        [SerializeField] TMP_Text pointsText;

        private EntityBaseStats stats;
        private int additions;

        public EntityBaseStats Stats => stats; 
        public bool HasAdditions => additions > 0;
        public bool NoAdditions => additions < 1;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartNewCharacter();
        }


        public void StartNewCharacter()
        {
            stats = new();
            foreach(StatAdjusterUI adjuster in statAdjusters) adjuster.SetParent(this);
            additions =  startingAdditions;
            pointsText.text = additions.ToString();
        }


        public void UpdateAdditions(int amount)
        {
            additions += amount;
            if(additions < 1)
            {
                foreach(StatAdjusterUI adjuster in statAdjusters) adjuster.HideIncrementButton();
            } 
            else
            {
                foreach(StatAdjusterUI adjuster in statAdjusters) adjuster.PossiblyShowIncrementButton();
                
            }
            pointsText.text = additions.ToString();
        }


        public void ApplyToCharacter(PCTalking pc)
        {
            pc.attributes.baseStats.CopyInto(stats);
            pc.attributes.DeriveAttributesForHuman(pc.health, pc.stamina, pc.mana);
        }






    }


}

