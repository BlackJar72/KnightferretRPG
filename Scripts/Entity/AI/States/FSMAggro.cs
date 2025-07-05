using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(menuName = "KF-RPG/AI/States/Hierarchal Aggro", fileName = "FSMAggro", order = 22)]
    public class FSMAggro : AIState
    {

        [SerializeField] AISubState see;
        [SerializeField] AISubState chase;
        [SerializeField] AISubState melee;
        [SerializeField] AISubState ranged;

        private AISubState[] stateArray;

        public AISubState currentState;


        public override void Init(EntityActing character)
        {
            base.Init(character);
            stateArray = new AISubState[4];
            see = Object.Instantiate(see);
            see.Init(owner, this);
            stateArray[0] = see;
            chase = Object.Instantiate(chase);
            chase.Init(owner, this);
            stateArray[1] = chase;
            melee = Object.Instantiate(melee);
            melee.Init(owner, this);
            stateArray[2] = melee;
            ranged = Object.Instantiate(ranged);
            ranged.Init(owner, this);
            stateArray[3] = see;
        }



        public override void Act()
        {
            currentState.Act();
        }


        /// <summary>
        /// Initialize the current state to the see state.
        /// 
        /// The see state should change the current state to chase (or whatever is appropriate) with its StateExit() method.
        /// </summary>
        public override void StateEnter()
        {
            currentState = see;
        }



        public override void StateExit()
        {
            currentState.StateExit();
        }


        public void SetStateNumber(int number) => currentState = stateArray[number];



    }

}
