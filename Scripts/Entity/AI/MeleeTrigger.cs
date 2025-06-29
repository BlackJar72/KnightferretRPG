using UnityEngine;


namespace kfutils.rpg
{

    public class MeleeTrigger : MonoBehaviour
    {
        private EntityActing owner;
        private bool triggered = false;
        private INotifiableT<MeleeTrigger> notifiable;

        public bool Triggered => triggered;
        public INotifiableT<MeleeTrigger> Notifiable { get => notifiable; set => notifiable = value; }


        public void Init(EntityActing entity) {
            if(owner == null) owner = entity;
        }


        void OnTriggerEnter(Collider other)
        {
            EntityLiving living = other.GetComponent<EntityLiving>();
            if (living == owner.targetEnemy)
            {
                triggered = true;
                notifiable.BeNotified(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            EntityLiving living = other.GetComponent<EntityLiving>();
            if (living == owner.targetEnemy)
            {
                triggered = false;
                notifiable.BeNotified(this);
            }      
        }


    }


}