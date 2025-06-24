using System;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, IActor
    {
        public const float VRANGESQR = 64 * 64;

        [SerializeField] CharacterInventory inventory;
        [SerializeField] Spellbook spellbook;
        [SerializeField] CharacterEquipt itemLocations;

        [SerializeField] AIStates basicStates;
        [SerializeField] AIStateID defaultState;
        [SerializeField] Alignment alignment = Alignment.neutral;

        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;

        public AnimancerLayer ActionLayer => actionLayer;
        public AnimancerState ActionState => actionState;

        public AIStates BasicStates => basicStates;
        public AIStateID DefaultState => defaultState;

        public Alignment AL => alignment;



        protected override void Awake()
        {
            base.Awake();
            basicStates.Init(this);
        }


        //Update is called once per frame
        protected override void Update()
        {
            basicStates.Act();
            base.Update();
        }


        protected override void Start()
        {
            base.Start();
            // Line only to 
            if (alive)
            {
                SetMoveType(MoveType.walk); // REMOVE ME?
            }
        }


        public void EquiptItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }


        public virtual void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
        }

        public void PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {
            throw new NotImplementedException();
        }

        public void PlayAction(AvatarMask mask, ITransition animation, Action onEnd, float time = 0, float delay = 1.0f)
        {
            throw new NotImplementedException();
        }

        public void PreSaveEquipt()
        {
            // TODO: This
            Debug.Log(ID + " => PreSaveEquipt()");
        }

        public void RemoveEquiptAnimation()
        {
            throw new System.NotImplementedException();
        }


        public void UnequiptItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }


        public void UnequiptItem(EEquiptSlot slot)
        {
            throw new System.NotImplementedException();
        }


        protected override void Die()
        {
            base.Die();
            basicStates.SetState(AIStateID.death);
        }


        /// <summary>
        /// Determines if a point could be seen.  This creates a hemisphere rather than 
        /// a cone of vision, with good periferal vision assumed.
        /// 
        /// To do this first it is tested to be in range (this can be thought of as the, 
        /// aggro range, though called a visual range).  Next is is determined if the 
        /// point is in front of the viewer based on the relationship between the dot 
        /// product and cosine (law of cosines) by which anything on the front side 
        /// will have positive cosine and thus positive dot product (as distance are 
        /// always positive).  Finally, live of site is tested to make sure the view 
        /// is not blocked.  The order of thes operations is based on least computationally 
        /// expensive to most, so as not to waste cpu cycles, implemented through the 
        /// languages own short-circuiting of compond logic statements.
        /// 
        /// It would be possible to get the actual cosine be dividing the dot product by 
        /// the distance (as the over vector in the dot product is a unit vector), but 
        /// this is not desired as periferal vision seems surprisingly limited even with a  
        /// hemisphere.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanSeePosition(Vector3 other)
        {
            Vector3 toOther = other - eyes.position;
            float dist = toOther.sqrMagnitude;
            return ((dist < VRANGESQR)
            && (Vector3.Dot(eyes.forward, toOther) > 0)
            && !Physics.Linecast(eyes.position, other, GameConstants.LevelMask));
        }
        public bool CanSeeTransform(Transform other) => CanSeePosition(other.position);
        public bool CanSeeCollider(Collider other) => CanSeePosition(other.bounds.center);
        public bool CanSeeEntity(EntityLiving other) => other.CanBeSeenFrom(eyes, VRANGESQR);

        
    }


}
