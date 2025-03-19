using UnityEngine;


namespace kfutils.rpg {

    public abstract class EntityLiving : MonoBehaviour, IHaveName 
    {
        [SerializeField] string entityName;        
        public EntityHealth health;
        public EntityStamina stamina;
        public EntityMana mana;
        public EntityAttributes attributes;


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
    
    
    
    }


}