using UnityEngine;


namespace kfutils.rpg {

    public class MovingData
    {

        public Vector3 movement;
        public Vector3 heading;
        public Quaternion rotation;
        public Vector3 lastPos;
        public float speed;
        public Vector3 hVelocity;
        public float vSpeed;
        public Vector3 velocity;
        public bool falling;
        public bool onGround;
        public TransformData navSeekerPos; 
        
        

    }

}
