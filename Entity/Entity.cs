using UnityEngine;


namespace kfutils.rpg
{

    public abstract class Entity : MonoBehaviour
    {
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

    }


}