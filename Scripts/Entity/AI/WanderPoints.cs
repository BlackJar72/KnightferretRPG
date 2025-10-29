using UnityEngine;


namespace kfutils.rpg
{


    public class WanderPoints : MonoBehaviour
    {
        [SerializeField] Transform[] waypoints;

        public Transform[] Waypoints => waypoints;


        public Transform GetWanderPoint()
        {
            return waypoints[Random.Range(0, waypoints.Length)];
        }
    }


}
