using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, IActor, IAttacker
    {
        public const float VRANGESQR = 64 * 64;

        [SerializeField] CharacterInventory inventory;
        [SerializeField] Spellbook spellbook;
        [SerializeField] CharacterEquipt itemLocations;

        [SerializeField] AIStates basicStates;
        [SerializeField] AIStateID defaultState;
        [SerializeField] Disposition disposition = Disposition.neutral;
        [SerializeField] MeleeTrigger meleeCollider;
        [SerializeField] Transform aimFrom;
        [SerializeField][Range(0.0f, 1.0f)] float aimAccuracy = 0.9f;

        [HideInInspector] public EntityLiving targetEnemy;

        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;

        public AnimancerLayer ActionLayer => actionLayer;
        public AnimancerState ActionState => actionState;

        public AIStates BasicStates => basicStates;
        public AIStateID DefaultState => defaultState;

        public Disposition AL => disposition;

        public MeleeTrigger meleeTrigger => meleeCollider;



        protected override void Awake()
        {
            base.Awake();
            meleeCollider.Init(this);
            basicStates.Init(this);
            inventory.SetOwner(this);
        }


        protected override void StoreData()
        {
            base.StoreData();
            data.actingData = new();
            data.actingData.disposition = disposition;
            if(targetEnemy == null) data.actingData.targetEnemy =  "";
            else data.actingData.targetEnemy =  targetEnemy.ID;
        }


        protected override void LoadData()
        {
            base.LoadData();
            InventoryManagement.loadNPCInventoryData += LoadInventoryData;
            inventory.OnEnable();
            disposition = data.actingData.disposition;
            // TODO: Find entity, if any, that has aggro
        }


        protected virtual void LoadInventoryData()
        {/*
            Debug.Log("protected virtual void LoadInventoryData() for " + ID);
            inventory.FixEquipt();
            inventory.OnEnable();            
        */}


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
            actionLayer = animancer.Layers[1];
            actionState = moveState;
            if (alive)
            {
                SetMoveType(MoveType.walk); // REMOVE ME?
            }
        }


        public void EquiptItem(ItemStack item)
        {
            if(item != null) {
                ItemEquipt equipt = itemLocations.EquipItem(item);
                IUsable usable = equipt as IUsable;
                if(usable != null) {                     
                    usable.OnEquipt(this); 
                }            
            } 
        }


        public void SetAimFrom(Transform t) => aimFrom = t;
        public void UnsetAimFrom() => aimFrom = eyes;


        public virtual void GetAimParams(out AimParams aim)
        {
            aim.from = aimFrom.position;
            aim.toward = (targetEnemy.GetComponent<Collider>().bounds.center - aimFrom.position).normalized;

            float magnitude, rotation, x, y;
            Quaternion scatter;
            magnitude = (Random.Range(0, 1 - aimAccuracy) - Random.Range(0, 1 - aimAccuracy)) * 90;
            rotation = Random.Range(0, 360);
            x = Mathf.Sin(rotation) * magnitude;
            y = Mathf.Cos(rotation) * magnitude;
            scatter = Quaternion.AngleAxis(x, aimFrom.right)
                    * Quaternion.AngleAxis(y, aimFrom.up);
            aim.toward = scatter * aim.toward;
        }


        public void PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {            
            actionLayer.SetMask(mask);
            actionState = animancer.Play(animation);
            actionState.Time = time;
        }


        public void PlayAction(AvatarMask mask, ITransition animation, System.Action onEnd, float time = 0, float delay = 1.0f)
        {
            actionLayer.SetMask(mask);
            actionState = actionLayer.Play(animation);
            actionState.Time = time;
            StartCoroutine(DoPostActionCode(onEnd, delay));
        }


        public IEnumerator DoPostActionCode(System.Action onEnd, float delay = 1.0f)
        {
            yield return new WaitForSeconds(delay);
            onEnd();
        }


        public void PreSaveEquipt()
        {
            inventory.Equipt.PreSave();
        }


        public void RemoveEquiptAnimation()
        {
            actionLayer?.StartFade(0);
        }


        public void UnequiptItem(ItemStack item)
        {
            if(item != null) {
                itemLocations.UnequipItem(item);
            } 
        }


        public void UnequiptItem(EEquiptSlot slot)
        {
            itemLocations.UnequipItem(slot);
        }


        protected override void Die()
        {
            base.Die();
            basicStates.SetState(AIStateID.death);
        }


        public virtual void MeleeAttack()
        {
            ItemEquipt requipt = itemLocations.GetRHandItem();
            if (requipt is IWeapon weapon) MeleeAttack(weapon);
        }


        public virtual void MeleeAttack(IWeapon weapon)
        {
            if (stamina.UseStamina(weapon.StaminaCost))
            {
                weapon.OnUse(this);
            }
        }


        public void RangedAttack(IWeapon weapon, Vector3 direction)
        {
            throw new System.NotImplementedException();
        }


        public void Block()
        {
            throw new System.NotImplementedException();
        }


        public void DrawWeapon(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }


        public void SheatheWeapon(IWeapon weapon)
        {
            throw new System.NotImplementedException();
        }


        public void SwitchWeapon(IWeapon currentWeapon, IWeapon newWeapon)
        {
            throw new System.NotImplementedException();
        }


        public void AttackBlocked()
        {
            throw new System.NotImplementedException();
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
