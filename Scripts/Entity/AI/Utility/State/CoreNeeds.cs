using UnityEngine;


namespace kfutils.rpg
{

    /*****************************************************************************************************
    //•   Energy: (Physical) Decreases somewhat slowly, and is restored by rest (sleep); exertion may
    //    drain it faster. Running out causes passing out.
    //
    //•   Hunger: (Physical) Decreases at a moderate pace and is restored by eating. Running out causes
    //    death.
    //
    //•   Social: (Psychological) Decreases over time at moderate speed, increased be socialization; rates
    //    in each direction are effected by Extroversion and by some extroverted Minor Traits. When low
    //    an emotion in the pure negative direction is produced (thus it can effect the next need, they are
    //    not completely orthogonal.)
    //
    //•   Health: (Physical) Based on several thing; slowly decreased when other physical needs are low,
    //    when diet is not balanced, or when sick or injured. Improved slowly by keeping other physical
    //    needs high and when recovering from illness. Running out causes death.
    //
    //•   Emotional: (Psychological) Based on positivity component of the current emotional state vector.
    //    When very low it become hard to continue or focus on tasks, so they may be abandoned
    //    prematurely.
    ********************************************************************************************************/

    [System.Serializable]
    public class CoreNeeds
    {

        [SerializeField] Need energy = new Need(0.8f, 1.2f, 0.9f);
        [SerializeField] Need nourishment = new Need(0.75f, 1.5f, 0.8f);
        [SerializeField] Need social = new Need(0.25f, 1.0f);
        [SerializeField] Need enjoyment = new Need(0.5f, 1.0f);

        public ITalker character;

        private Need[] allNeeds;


        public void Init(EntityTalking owner)
        {
            character = owner;
            allNeeds = new Need[] { energy, nourishment, social, enjoyment };
        }


        public Need GetNeed(ENeed need) => allNeeds[(int)need];


        public void UpdateNeeds()
        {
            energy.Decay();
            nourishment.Decay();
            social.Decay();
            enjoyment.Decay();
        }


        public void AlterNeedGradual(ENeed need, float timeForEffect, float amount)
        {
            Need theNeed = allNeeds[(int)need];
            theNeed.Add((amount / timeForEffect) * Time.deltaTime);
        }


        public void AlterNeedInstant(ENeed need, float amount)
        {
            Need theNeed = allNeeds[(int)need];
            theNeed.Add(amount);
        }
        


    }



}
