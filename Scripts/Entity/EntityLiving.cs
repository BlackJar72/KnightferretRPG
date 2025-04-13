using UnityEngine;
using Animancer;


namespace kfutils.rpg {

    public abstract class EntityLiving : MonoBehaviour, IHaveName, IHaveStringID, IDamageable  
    {

        [SerializeField] private string id; // Should this be a string? A number? A UUID?  String for now.  Also, this must never change!
        [SerializeField] protected string entityName; // A name probably shared by all entities of a type (some my also have personal names separately.)

        [SerializeField] public EntityHealth health;
        [SerializeField] public EntityStamina stamina;
        [SerializeField] public EntityMana mana;
        [SerializeField] public EntityAttributes attributes;

        [SerializeField] protected AnimancerComponent animancer;


        public AnimancerComponent anim { get => animancer; }


        public string ID { get => id;
                           protected set { if(string.IsNullOrEmpty(id)) id = value; }  }
        // Seems sligihtly convoluted, but this should allow id to remain private while allowing for the PC to always have its ID
        protected virtual void MakePC(string id) { 
            if(this is not PCMoving) Debug.LogError("You are not allowed to reset this ID!"); 
            this.id = id; 
        }


        protected virtual void Awake() {
            health.SetOnwer(this);
            stamina.SetOnwer(this);
            mana.SetOnwer(this);
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {

        }


        // Update is called once per frame
        protected virtual void Update()
        {
            
        }


        public virtual string GetName() => entityName;
        public virtual string GetPersonalName() => GetName();


        public virtual int GetArmor() {
            return attributes.naturalArmor;
        }


        public void TakeDamage(Damages damage) {
            health.TakeDamage(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, attributes.damageAdjuster)));
        }


        public virtual void TakeDamage(DamageData damage) {
            health.TakeDamage(attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage.damage, attributes.damageAdjuster)));
            // TODO: Include overrides that can react to the IAttacker
        }
    
    
    
    }


}
