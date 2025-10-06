using System;
using Animancer;
using UnityEngine;


namespace kfutils.rpg
{

    /// <summary>
    /// An alternative to EntityActing for non-humanoid creatures
    /// 
    /// TODO!!!
    /// 
    /// </summary>
    public class EntityMob : EntityMoving, ICombatantAI
    {


        // Serialized Fields
        [SerializeField] protected EntitySounds entitySounds;
        [SerializeField] protected AudioSource voice;
        [SerializeField] protected float aggroRange = 64.0f;
        [SerializeField] protected float attackTime = 1.0f;

        protected float aggroRangeSq;
        protected float nextAttack;
        protected float nextIdleTalk;
        protected float meleeStopDistance;

        public bool IsBlocking => throw new NotImplementedException();

        public AnimancerLayer ActionLayer => throw new NotImplementedException();

        public AnimancerState ActionState => throw new NotImplementedException();

        public CharacterInventory CharInventory => throw new NotImplementedException();



        #region  Overridden Unity Methods

        protected override void Awake()
        {
            base.Awake();
            aggroRangeSq = aggroRange * aggroRange;
        }


        protected override void Start()
        {
            base.Start();
        }


        protected override void Update()
        {
            base.Update();
        }


        protected override void OnEnable()
        {
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }

        #endregion


        public void MeleeAttack(IWeapon weapon)
        {
            throw new NotImplementedException();
        }


        public void DrawWeapon(IWeapon weapon)
        {
            throw new NotImplementedException();
        }


        public void SheatheWeapon(IWeapon weapon)
        {
            throw new NotImplementedException();
        }


        public void SwitchWeapon(IWeapon currentWeapon, IWeapon newWeapon)
        {
            throw new NotImplementedException();
        }


        public void RangedAttack(IWeapon weapon, Vector3 direction)
        {
            throw new NotImplementedException();
        }


        public void Block(ItemEquipt item)
        {
            throw new NotImplementedException();
        }


        public void EndBlock(ItemEquipt item)
        {
            throw new NotImplementedException();
        }


        public void BlockDamage(Damages damage, BlockArea blockArea)
        {
            throw new NotImplementedException();
        }


        public void BlockDamage(DamageData damage, BlockArea blockArea)
        {
            throw new NotImplementedException();
        }


        public BlockArea GetBlockArea()
        {
            throw new NotImplementedException();
        }


        public void EquiptItemToBody(ItemStack item)
        {
            throw new NotImplementedException();
        }


        public void UnequiptItemFromBody(ItemStack item)
        {
            throw new NotImplementedException();
        }


        public void UnequiptItemFromBody(EEquiptSlot slot)
        {
            throw new NotImplementedException();
        }


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, float time = 0)
        {
            throw new NotImplementedException();
        }


        public AnimancerState PlayAction(AvatarMask mask, ITransition animation, Action onEnd, float time = 0, float delay = 1)
        {
            throw new NotImplementedException();
        }


        public void StopAction()
        {
            throw new NotImplementedException();
        }


        public void RemoveEquiptAnimation()
        {
            throw new NotImplementedException();
        }


        public void GetAimParams(out AimParams aim)
        {
            throw new NotImplementedException();
        }


        public void PreSaveEquipt()
        {
            throw new NotImplementedException();
        }






    }

}
