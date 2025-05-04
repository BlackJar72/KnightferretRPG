using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class PCLiving : AbstractEntity, IHaveName, IHaveStringID, IDamageable {


        [SerializeField] private string id;

        [SerializeField]  EntityHealth health;
        [SerializeField]  EntityStamina stamina;
        [SerializeField]  EntityMana mana;
        [SerializeField]  EntityAttributes attributes;
       
        [SerializeField] protected AnimancerComponent animancer;
        

        public override string ID { get => id;
                           protected set { if(string.IsNullOrEmpty(id)) id = value; }  }
        // Seems sligihtly convoluted, but this should allow id to remain private while allowing for the PC to always have its ID
        public override EntityHealth Health => health;
        public override EntityStamina Stamina => stamina;
        public override EntityMana Mana => mana;
        public override EntityAttributes Attributes => attributes;
        public override AnimancerComponent anim { get => animancer; }



        protected virtual void MakePC(string id) { 
            if(this is not PCMoving) Debug.LogError("You are not allowed to reset this ID!"); 
            this.id = id; 
        }


        protected virtual void Awake() {
            Health.SetOnwer(this);
            Stamina.SetOnwer(this);
            Mana.SetOnwer(this);
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {

        }


        // Update is called once per frame
        protected virtual void Update()
        {
            
        }


        public virtual string GetName() => data.l.entityName;
        public virtual string GetPersonalName() => GetName();


        public virtual int GetArmor() {
            return data.l.attributes.naturalArmor;
        }


        public void TakeDamage(Damages damage) {
            Health.TakeDamage(Attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage, Attributes.damageAdjuster)));
        }


        public virtual void TakeDamage(DamageData damage) {
            Health.TakeDamage(Attributes.damageModifiers.Apply(DamageAdjustList.Adjust(damage.damage, Attributes.damageAdjuster)));
            // TODO: Include overrides that can react to the IAttacker
        }
    
    
    
    }

}
