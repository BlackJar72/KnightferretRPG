using UnityEngine;


namespace kfutils.rpg
{
    
    [System.Serializable]
    public class Sound
    {
        [SerializeField] AudioClip sound;

        [Range(0f, 1f)]
        [SerializeField] float volume = 1;

        [Range (0.5f, 2f)]
        [SerializeField] float pitch = 1;

        public void Play(AudioSource source)
        {
            source.clip = sound;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }

        public void PlayRandomized(AudioSource source)
        {
            source.clip = sound;
            source.volume = Random.Range(0.5f, 1.0f);
            source.pitch = Random.Range(0.75f, 1.0f);
            source.Play();
        }
    }

}