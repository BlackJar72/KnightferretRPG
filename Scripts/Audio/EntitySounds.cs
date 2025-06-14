using UnityEngine;


namespace kfutils.rpg
{
    public enum ESoundType
    {
        IDLE = 0,
        AGGRO = 1,
        ATTACK = 2,
        PAIN = 3,
        DEATH = 4
    }


    [CreateAssetMenu(menuName = "RPG/Audio/Entity Sounds", fileName = "EntitySounds", order = 103)]
    public class EntitySounds : ScriptableObject
    {
        [SerializeField] Sound[] idleSounds;
        [SerializeField] Sound[] aggroSounds;
        [SerializeField] Sound[] attackSounds;
        [SerializeField] Sound[] painSounds;
        [SerializeField] Sound[] deathSounds;


        public void PlaySound(AudioSource source, ESoundType type, int sound)
        {
            {
                switch (type)
                {
                    case ESoundType.IDLE:
                        PlayIdle(source, sound);
                        return;
                    case ESoundType.AGGRO:
                        PlayAggro(source, sound);
                        return;
                    case ESoundType.ATTACK:
                        PlayAttack(source, sound);
                        return;
                    case ESoundType.PAIN:
                        PlayHurt(source, sound);
                        return;
                    case ESoundType.DEATH:
                        PlayDeath(source, sound);
                        return;
                    default: return;
                }
            }
        }


        public void PlaySound(AudioSource source, int type, int sound)
        {
            {
                switch (type)
                {
                    case (int)ESoundType.IDLE:
                        PlayIdle(source, sound);
                        return;
                    case (int)ESoundType.AGGRO:
                        PlayAggro(source, sound);
                        return;
                    case (int)ESoundType.ATTACK:
                        PlayAttack(source, sound);
                        return;
                    case (int)ESoundType.PAIN:
                        PlayHurt(source, sound);
                        return;
                    case (int)ESoundType.DEATH:
                        PlayDeath(source, sound);
                        return;
                    default: return;
                }
            }
        }


        public void PlaySound(AudioSource source, ESoundType type)
        {
            {
                switch (type)
                {
                    case ESoundType.IDLE:
                        PlayIdle(source);
                        return;
                    case ESoundType.AGGRO:
                        PlayAggro(source);
                        return;
                    case ESoundType.ATTACK:
                        PlayAttack(source);
                        return;
                    case ESoundType.PAIN:
                        PlayHurt(source);
                        return;
                    case ESoundType.DEATH:
                        PlayDeath(source);
                        return;
                    default: return;
                }
            }
        }


        public void PlayIdle(AudioSource source, int sound)
        {
            if (!source.isPlaying)
            {
                idleSounds[sound].Play(source);
            }
        }


        public void PlayIdle(AudioSource source)
        {
            if (!source.isPlaying)
            {
                idleSounds[Random.Range(0, idleSounds.Length)].PlayRandomized(source);
            }
        }


        public void PlayAggro(AudioSource source, int sound)
        {
            source.Stop();
            aggroSounds[sound].Play(source);
        }


        public void PlayAggro(AudioSource source)
        {
            source.Stop();
            aggroSounds[Random.Range(0, aggroSounds.Length)].Play(source);
        }


        public void PlayAttack(AudioSource source, int sound)
        {
            if (!source.isPlaying)
            {
                attackSounds[sound].Play(source);
            }
        }


        public void PlayAttack(AudioSource source)
        {
            if (!source.isPlaying)
            {
                attackSounds[Random.Range(0, attackSounds.Length)].Play(source);
            }
        }


        public void PlayHurt(AudioSource source, int sound)
        {
            source.Stop();
            painSounds[sound].Play(source);
        }


        public void PlayHurt(AudioSource source)
        {
            source.Stop();
            painSounds[Random.Range(0, painSounds.Length)].Play(source);
        }


        public void PlayDeath(AudioSource source, int sound)
        {
            source.Stop();
            deathSounds[sound].Play(source);
        }


        public void PlayDeath(AudioSource source)
        {
            source.Stop();
            deathSounds[Random.Range(0, attackSounds.Length)].Play(source);
        }


        public void PlayDeathRandomized(AudioSource source)
        {
            attackSounds[Random.Range(0, attackSounds.Length)].PlayRandomized(source);
        }


        public bool HasSound(ESoundType type)
        {
            switch (type)
            {
                case ESoundType.IDLE:
                    return idleSounds.Length > 0;
                case ESoundType.AGGRO:
                    return aggroSounds.Length > 0;
                case ESoundType.ATTACK:
                    return attackSounds.Length > 0;
                case ESoundType.PAIN:
                    return painSounds.Length > 0;
                case ESoundType.DEATH:
                    return deathSounds.Length > 0;
                default: return false;
            }
        }


    }

}