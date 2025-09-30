using System.Collections;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class EntityActing : EntityMoving, ICombatantAI
    {
        public const float VRANGESQR = 64 * 64;

        [SerializeField] protected CharacterInventory inventory;
        [SerializeField] protected Spellbook spellbook;
        [SerializeField] protected CharacterEquipt itemLocations;

        [SerializeField] protected AIStates basicStates;
        [SerializeField] protected AIStateID defaultState;
        [SerializeField] protected Disposition disposition = Disposition.neutral;
        [SerializeField] protected MeleeTrigger meleeCollider;
        [SerializeField] Transform aimFrom;
        [SerializeField][Range(0.0f, 1.0f)] float aimAccuracy = 0.9f;

        [HideInInspector] public EntityLiving targetEnemy;

        protected AnimancerLayer actionLayer;
        protected AnimancerState actionState;
        protected float updateEnd = float.NegativeInfinity;
        protected bool isParried;

        protected bool blocking;

        public AnimancerLayer ActionLayer => actionLayer;
        public AnimancerState ActionState => actionState;

        public AIStates BasicStates => basicStates;
        public AIStateID DefaultState => defaultState;

        public Disposition AL => disposition;

        public MeleeTrigger meleeTrigger => meleeCollider;
        public CharacterInventory CharInventory => inventory;
        public bool IsBlocking => blocking;


        protected delegate void ActionUpdate();
        protected ActionUpdate actionUpdate;



        protected override void Awake()
        {
            base.Awake();
            meleeCollider.Init(this);
            basicStates.Init(this);
            inventory.SetOwner(this);
            actionUpdate = NormalUpdate;
        }


        protected override void StoreData()
        {
            base.StoreData();
            data.actingData = new();
            data.actingData.disposition = disposition;
            if (targetEnemy == null) data.actingData.targetEnemy = "";
            else data.actingData.targetEnemy = targetEnemy.ID;
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
        */
        }


        //Update is called once per frame
        protected override void Update()
        {
            actionUpdate();
        }


        protected void NormalUpdate()
        {
            basicStates.Act();
            base.Update();
        }


        protected void DelayedUpdate()
        {
            if (Time.time > updateEnd)
            {
                actionUpdate = NormalUpdate;
                isParried = false;
            }
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
            if (item != null)
            {
                ItemEquipt equipt = itemLocations.EquipItem(item);
                IUsable usable = equipt as IUsable;
                if (usable != null)
                {
                    usable.OnEquipt(this);
                }
            }
        }


        public override bool IsSurprised(ICombatant attacker)
        {
            EntityLiving attackingEntity = attacker as EntityLiving;
            return !basicStates.IsAggro && (attackingEntity != null) && !CanSeeEntity(attackingEntity);
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


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {
            actionLayer.SetMask(mask);
            actionState = actionLayer.Play(animation);
            actionState.Time = time;
            return actionState;
        }


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, System.Action onEnd, float time = 0, float delay = 1.0f)
        {
            actionLayer.SetMask(mask);
            actionState = actionLayer.Play(animation);
            actionState.Time = time;
            StartCoroutine(DoPostActionCode(onEnd, delay));
            return actionState;
        }


        public void StopAction()
        {
            actionLayer.StartFade(0, 0.1f);
        }


        public void Stagger(float delay)
        {
            if (alive)
            {
                StopAction();
                moveState = moveLayer.Play(movementSet.Staggered);
                updateEnd = Time.time + delay;
                actionUpdate = DelayedUpdate;
            }
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


        public override void TakeDamage(DamageData damage)
        {
            base.TakeDamage(damage);
            if (isParried) updateEnd = Mathf.Min(updateEnd, Time.time + 0.5f); 
            isParried = false;
        }


        public void UnequiptItem(ItemStack item)
        {
            if (item != null)
            {
                itemLocations.UnequipItem(item);
            }
        }


        public void UnequiptItem(EEquiptSlot slot)
        {
            itemLocations.UnequipItem(slot);
        }


        protected override void Die()
        {
            actionUpdate = NormalUpdate;
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


        public override bool IsStunned()
        {
            return actionUpdate == DelayedUpdate;
        }


        public override bool InParriedState() => isParried;


        public override void SetParried(bool parried = true) => isParried = parried;


        public void Block(ItemEquipt item)
        {
            throw new System.NotImplementedException();
        }


        public void EndBlock(ItemEquipt item)
        {
            throw new System.NotImplementedException();
        }


        public void BlockDamage(Damages damage, BlockArea blockArea)
        {
            throw new System.NotImplementedException();
        }


        public void BlockDamage(DamageData damage, BlockArea blockArea)
        {
            throw new System.NotImplementedException();
        }


        public void RangedAttack(IWeapon weapon, Vector3 direction)
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


        public BlockArea GetBlockArea()
        {
            return null; // FIXME; Return an actual block are if there is one
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
