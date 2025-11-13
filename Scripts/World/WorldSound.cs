using UnityEngine;



namespace kfutils.rpg {


    /// <summary>
    /// This represents a sound in world, which may be heard by NPCs (including enemies). 
    /// 
    /// It has nothing to do with sounds heard by the player, so nothing to do with playing 
    /// sounds through the hardware; for that use the Sound.cs class under Audio.
    /// </summary>
    public class WorldSound
    {
        private Vector3 location;
        private float range; // The range at which it may be heard 

        public Vector3 Location => location;
        public float Loudness => range;


        public WorldSound(Vector3 position, float loudness)
        {
            location = position;
            range = loudness;
        }


        public WorldSound(Transform transform, float loudness)
        {
            location = transform.position;
            range = loudness;
        }

    }


}
