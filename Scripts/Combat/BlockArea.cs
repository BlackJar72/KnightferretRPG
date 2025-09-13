
using UnityEngine;


namespace kfutils.rpg
{


    public class BlockArea : MonoBehaviour
    {

        [SerializeField] BoxCollider bc;
        [SerializeField] Rigidbody rb;

        ICombatant owner;

        public IBlockItem blockItem;

        float blockTime = float.NegativeInfinity;

        public float BlockTime => blockTime;




        void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
        }


        public void SetOwner(ICombatant combatant)
        {
            owner = combatant;
        }


        public void RaiseBlock(IBlockItem item)
        {
            blockItem = item;
            blockTime = Time.time;
            gameObject.SetActive(true);
        }


        public void LowerBlock()
        {
            gameObject.SetActive(false);
        }


        void OnTriggerEnter(Collider other)
        {
            IWeapon weapon = other.gameObject.GetComponent<IWeapon>();
            if (weapon != null)
            {
                weapon.BeBlocked(owner, this);
            }
        }



    }


}