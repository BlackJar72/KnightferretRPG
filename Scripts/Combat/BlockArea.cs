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


        void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Block Area Hit!");
        }


        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Block Area Entered");          
        }



    }


}