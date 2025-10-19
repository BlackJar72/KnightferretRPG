using UnityEngine;
using Animancer;


namespace kfutils.rpg {

    public abstract class EntityLiving : MonoBehaviour, IHaveName, IHaveStringID, IDamageable, IInWorld 
    {

        [SerializeField] private string id; // Should this be a string? A number? A UUID?  String for now.  Also, this must never change!
        [SerializeField] protected string entityName; // A name probably shared by all entities of a type (some my also have personal names separately.)

        [SerializeField] public EntityHealth health;
        [SerializeField] public EntityStamina stamina;
        [SerializeField] public EntityMana mana;
        [SerializeField] public EntityAttributes attributes;
        [SerializeField] protected bool alive = true;

        [SerializeField] protected EntityHitbox hitbox;

        [SerializeField] protected AnimancerComponent animancer;

        [SerializeField] protected AbstractAction deathAnimation;

        protected EntityData data;

        [ES3Serializable] protected float enviroCooldown;

        public AnimancerComponent anim { get => animancer; }
        public bool Alive => alive;
        public EntityHitbox Hitbox => hitbox;
        public EntityLiving GetEntity => this;

        public ChunkManager GetChunkManager => WorldManagement.WorldLogic.GetChunk(transform);


        public virtual string GetName() => entityName;
        public virtual string GetPersonalName() => GetName();


        public string ID
        {
            get => id;
            protected set { if (string.IsNullOrEmpty(id)) id = value; }
        }


        // Seems sligihtly convoluted, but this should allow id to remain private while allowing for the PC to always have its ID
        protected virtual void MakePC(string id)
        {
            if (this is not PCMoving) Debug.LogError("You are not allowed to reset this ID!");
            else this.id = id;
        }


        public virtual void CopyInto(object other)
        {
            EntityLiving living = other as EntityLiving;
            if (living != null)
            {

            }
        }


        protected virtual void Awake()
        {
            health.SetOnwer(this);
            stamina.SetOnwer(this);
            mana.SetOnwer(this);
        }


        protected virtual void Start()
        {
            health.SetOnwer(this);
            stamina.SetOnwer(this);
            mana.SetOnwer(this);
        }


        protected virtual void OnEnable()
        {
            data = EntityManagement.GetFromRegistry(id);
            if (data == null)
            {
                data = new(id);
                //ExtraInit();
                StoreData();
                EntityManagement.AddToRegistry(data);
            }
            else
            {
                LoadData();
            }
            if (!alive && this is not EntityMoving) Die();
        }


        protected virtual void OnDisable()
        {
            StoreData();
        }


        //protected virtual void ExtraInit() {}


        protected virtual void StoreData()
        {
            data.livingData ??= new();
            data.livingData.entityName = entityName;
            data.livingData.attributes = attributes;
            data.livingData.health = health;
            data.livingData.stamina = stamina;
            data.livingData.mana = mana;
            data.livingData.alive = alive;
            data.livingData.enviroCooldown = enviroCooldown;
            data.livingData.transform = transform.GetGlobalData();
        }


        protected virtual void LoadData()
        {
            entityName = data.livingData.entityName;
            attributes = data.livingData.attributes;
            health = data.livingData.health;
            stamina = data.livingData.stamina;
            mana = data.livingData.mana;
            alive = data.livingData.alive;
            enviroCooldown = data.livingData.enviroCooldown;
            transform.SetDataGlobal(data.livingData.transform);
        }


        // Update is called once per frame
        protected virtual void Update()
        {

        }



        public virtual int GetArmor()
        {
            return attributes.naturalArmor;
        }


        public void TakeDamage(Damages damage)
        {
            health.TakeDamage(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
        }


        public virtual void TakeDamage(DamageData damage)
        {
            health.TakeDamage(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage.damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
            // TODO: Include overrides that can react to the IAttacker
        }

        public void HealDamage(float amount)
        {
            health.Heal(amount);
        }

        public Damages ApplyDamageAdjustment(Damages damage)
        {
            return attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, attributes.damageAdjuster));
        }


        protected virtual void Die()
        {
            alive = false;
            if (hitbox != null) hitbox.gameObject.SetActive(false);
            EntityManagement.RemoveDead(this);
            OnDisable();
        }


        public virtual bool CanBeSeenFrom(Transform from, float rangeSqr)
        {
            if (hitbox == null) return false;
            Vector3 toOther = hitbox.GetCenter() - from.position;
            float dist = toOther.sqrMagnitude;
            return ((dist < rangeSqr)
            && (Vector3.Dot(from.forward, toOther) > 0)
            && !Physics.Linecast(from.position, hitbox.GetCenter(), GameConstants.LevelMask));
        }


        public virtual bool IsStunned()
        {
            return false; // May change
        }


        public virtual bool InParriedState()
        {
            return false; // Could change (but might not)
        }


        public virtual void SetParried(bool parried = true)
        {
            // Do Nothing, at least for now
        }


        public virtual bool IsSurprised(ICombatant attacker) => false;




    }


}
