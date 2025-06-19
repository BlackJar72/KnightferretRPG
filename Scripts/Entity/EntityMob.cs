using UnityEngine;


namespace kfutils.rpg
{

    public class EntityMob : EntityActing
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


        



    }

}
