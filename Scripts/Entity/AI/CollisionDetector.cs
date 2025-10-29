using UnityEngine;


namespace kfutils.rpg
{

    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] EntityMoving owner;


        void OnTriggerEnter(Collider other)
        {
            Debug.Log(owner.ID + " collided with " + other.gameObject);
        }


    }

}
