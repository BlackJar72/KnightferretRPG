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


        public void Randomize()
        {
            stats.GenRandomHumanStats();
            foreach(StatAdjusterUI adjuster in statAdjusters) {
                adjuster.UpdateText();
                adjuster.HideIncrementButton();
                adjuster.HideDecrementButton();
            }
            additions = 0;
            pointsText.text = "0";
        }


        public void ResetStats()
        {
            StartNewCharacter();
            foreach(StatAdjusterUI adjuster in statAdjusters) {
                adjuster.ShowIncrementButton();
                adjuster.ShowDecrementButton();
            }
        }






    }


}

