using UnityEngine;



namespace kfutils.rpg {

    public enum SoundType
    {
        General = 0,  // Most sounds, including player movement and attacks 
        Alert = 1,    // Alerts from faction members (could include alarms too) 
        Pain = 2,     // Pain sounds from creatures, who may make them even while dying
        Speech = 3,   // Directed speech, especially talking to the hearer
        Music = 4,    // Music, you know what it is
        Strange = 5,  // Weird sounds that might leave someone wondering what it is
        Dramatic = 6  // Loud, dangerous, or otherwise attention grabbing sounds  
    }


    public interface ISoundSource
    {
        public void MakeSound(float loudness, SoundType soundType = SoundType.General);
    }


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
        private SoundType type;
        private ISoundSource source; // The game object the sound came from

        public Vector3 Location => location;
        public float Loudness => range;
        public SoundType Type => type;
        public ISoundSource Source => source;


        public WorldSound(Vector3 position, float loudness, SoundType soundType = SoundType.General, ISoundSource src = null)
        {
            location = position;
            range = loudness;
            type = soundType;
            source = src;
        }


        public WorldSound(Transform transform, float loudness, SoundType soundType = SoundType.General, ISoundSource src = null)
        {
            location = transform.position;
            range = loudness;
            type = soundType;
            source = src;
        }

    }


}
