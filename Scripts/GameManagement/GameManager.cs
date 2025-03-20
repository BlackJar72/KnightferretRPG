using UnityEngine;



namespace kfutils.rpg
{

    public class GameManager : MonoBehaviour
    {

        void Awake()
        {
            EntityManagement.Initialize();
        }

        /*// Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }*/

        // Update is called once per frame
        void Update()
        {
            EntityManagement.Update();
            // Other game management stuff will go here
        }


    }

    

}
