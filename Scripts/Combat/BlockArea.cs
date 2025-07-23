using UnityEngine;


namespace kfutils.rpg
{


    public class BlockArea : MonoBehaviour
    {

        [SerializeField] BoxCollider bc;
        [SerializeField] Rigidbody rb;
        



        void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
        }


    }


}