using UnityEngine;
using Animancer;


namespace kfutils.rpg {

    public enum Alertness
    {
        Comatose = -2,
        Unconscious = -1,
        Oblivious = 0,
        Noticing = 1,
        Suspicious = 2,
        Alerted = 3
    }


    public abstract class EntityLiving : MonoBehaviour, IHaveName, IHaveStringID, IDamageable, IInWorld 
    {

        [SerializeField] private string id; // Should this be a string? A number? A UUID?  String for now.  Also, this must never change!
        [SerializeField] protected string entityName; // A name probably shared by all entities of a type (some my also have personal names separately.)

        [SerializeField] public EntityHealth health;
        [SerializeField] public EntityStamina stamina;
        [SerializeField] public EntityMana mana;
        [SerializeField] public EntityAttributes attributes;
        [SerializeField] public StatusEffects statusEffects;
        [SerializeField] protected bool alive = true;
        [SerializeField] protected Alertness alertness = Alertness.Oblivious;

        [SerializeField] protected EntityHitbox hitbox;

        [SerializeField] protected AnimancerComponent animancer;

        [SerializeField] protected AbstractAction deathAnimation;

        protected EntityData data; 


        protected float enviroCooldown; // I might still use this to determine something like pain sounds

        public AnimancerComponent anim { get => animancer; }
        public bool Alive => alive;
        public Alertness Awareness => alertness;
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
            if (alive) EntityManagement.activeEntities.Add(this);
        }


        protected virtual void OnDisable()
        {
            EntityManagement.activeEntities.Remove(this);
            StoreData();
        }


        //protected virtual void ExtraInit() {}

        public void StoreDataForSave() => StoreData();
        protected virtual void StoreData()
        {
            data.livingData ??= new();
            data.livingData.entityName = entityName;
            data.livingData.attributes = attributes;
            data.livingData.health = health;
            data.livingData.stamina = stamina;
            data.livingData.mana = mana;
            data.livingData.alive = alive;
            data.livingData.statusEffects = statusEffects;
            data.livingData.enviroCooldown = enviroCooldown;
            data.livingData.transform = transform.GetGlobalData();
        }


        protected virtual void LoadData()
        {
            entityName = data.livingData.entityName;
            attributes = data.livingData.attributes;
            health = data.livingData.health.BeLoaded(this);
            stamina = data.livingData.stamina.BeLoaded(this);
            mana = data.livingData.mana.BeLoaded(this);
            alive = data.livingData.alive;
            statusEffects = data.livingData.statusEffects;
            enviroCooldown = data.livingData.enviroCooldown;
            transform.SetDataGlobal(data.livingData.transform);
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            statusEffects.ApplyEffects(this);
        }


        public void FixOwnerships()
        {
            health.SetOnwer(this);
            stamina.SetOnwer(this);
            mana.SetOnwer(this);
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
        }


        public void TakeDamageOverTime(Damages damage) { 
            health.TakeDamageOverTime(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
        }


        public void TakeDamageOverTime(DamageData damage) { 
            health.TakeDamageOverTime(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage.damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
        }


        public void TakeShockOverTime(Damages damage) {
            health.TakeShockOverTime(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
        }


        public void TakeShockOverTime(DamageData damage) {
            health.TakeShockOverTime(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage.damage, attributes.damageAdjuster)));
            if (health.ShouldDie) Die();
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


        public void AddStatusEffect(StatusEffects.EEffectType type, float magnitude, double duration)
        {
            statusEffects.AddEffect(this, type, magnitude, duration);
        }


        public void AddItemStatusEffect(StatusEffects.EEffectType type, float magnitude, string id)
        {
            statusEffects.AddItemEffect(this, type, magnitude, id);
        }


        public void RemoveItemStatusEffect(string id, StatusEffects.EEffectType type)
        {
            statusEffects.RemoveEffect(this, id, type);
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


        public bool CanHear(WorldSound sound, out float howMuch)
        {
            // First, check the Euclidean distance, as this is cheap and will weed out a lot
            float distance =  (transform.position - sound.Location).magnitude;
            howMuch = sound.Loudness - distance;
            if(howMuch > 0)
            {
                // TODO: More advanced check after quick check (euclidean distance)
                //       We need to check if there is a path from the sound to here 
                //       that has a length less than the loudness. This should use 
                //       a special nav mesh for sounds, which should update for things
                //       like opening doors.  However, we may(?) limit this to dungeon 
                //       world spaces, and just use distance in the open world. 
                //
                //       It may also be a good idea to have an awareness variable set 
                //       which the AI scripts can reference and choose how (or if) 
                //       to react.  This will likely be called from the SoundManagement 
                //       which may have some other checks of its own.  Keep as much
                //       sound out of the living entities' code as possible. 
                howMuch /= distance; 
                howMuch *= howMuch; 
                return true; // FIXME!
            }
            howMuch = 0;
            return false;
        }


        public virtual void HearSound(WorldSound sound, float howMuch) {}




    }


}
