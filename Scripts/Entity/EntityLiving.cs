using UnityEngine;


namespace kfutils.rpg {

    public abstract class EntityLiving : MonoBehaviour, IHaveName 
    {
        [SerializeField] string entityName;        
        [SerializeField] public EntityHealth health;
        [SerializeField] public EntityStamina stamina;
        [SerializeField] public EntityMana mana;
        [SerializeField] public EntityAttributes attributes;


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