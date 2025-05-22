using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class PCData
    {
        // Basic Entity Data
        public EntityData entityData;

        // Movement Data   
        public TransformData location;
        public PCMoving.MoveMethod moveMethod;   
        public Vector3 movement;
        public PCMoving.MoveType moveType = PCMoving.MoveType.NORMAL;
        public float baseSpeed;
        public Vector3 hVelocity;
        public float vSpeed;
        public Vector3 velocity;
        public bool falling;
        public bool onGround;
        public bool shouldJump;
        public bool hasJumped;
        public bool shouldSprint;
        public bool shouldCrouch;
        public float weightMovementFactor = 1.0f;
        public float weightBoyancyFactor = 1.0f;
        public float looky;



    }


}