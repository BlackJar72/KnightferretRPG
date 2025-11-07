using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class StatusEffects
    {
        #region Class Core        
        [SerializeField] List<Effect> effects = new();


        public void AddEffect(EntityLiving entity, Effect effect)
        {
            if (!effects.Contains(effect))
            {
                effect.StartEffect(entity);
                effects.Add(effect);
            }
        }


        public void AddEffect(EntityLiving entity, EEffectType type, float magnitude, double duration)
        {
            Effect effect = new Effect(type, magnitude, duration);
            if (!effects.Contains(effect))
            {
                effect.StartEffect(entity);
                effects.Add(effect);
            }
        }


        public void AddItemEffect(EntityLiving entity, EEffectType type, float magnitude, string id)
        {
            Effect effect = new Effect(type, magnitude, id);
            if (!effects.Contains(effect))
            {
                effect.StartEffect(entity);
                effects.Add(effect);
            }
        }


        public void ApplyEffects(EntityLiving entity)
        {
            for (int i = effects.Count - 1; i > -1; i--)
            {
                effects[i].ContinueEffect(entity);
                if (effects[i].ShouldEnd)
                {
                    effects[i].EndEffect(entity);
                    effects.RemoveAt(i);
                }
            }
        }


        public void RemoveEffect(EntityLiving entity, string id, EEffectType type)
        {
            Effect toRemove = new(type, 0.0f, id);
            for (int i = effects.Count - 1; i > -1; i--)
            {
                if (effects[i] == toRemove)
                {
                    effects[i].EndEffect(entity);
                    effects.RemoveAt(i);
                    break;
                }
            }
        }


        #endregion Class Core



        [System.Serializable]
        public class Effect
        {
            [SerializeField] EEffectType type;
            [SerializeField] float magnitude;
            [SerializeField] double endTime;
            [SerializeField] bool finite;

            public EEffectType Type => type;
            public float Magnitude => magnitude;
            public double EndTime => endTime;
            public bool Finite => finite;
            public bool ShouldEnd => finite && (endTime < WorldTime.time);
            public string MakeID => type + endTime.ToString() + finite;

            public static bool operator ==(Effect a, Effect b) => (a.type == b.type) && (a.finite == b.finite) && (a.endTime == b.endTime);
            public static bool operator !=(Effect a, Effect b) => (a.type != b.type) || (a.finite != b.finite) || (a.endTime != b.endTime);

            public Effect()
            {
                type = (EEffectType)0;
                magnitude = 0.0f;
                endTime = 0.0;
                finite = true;
            }

            public Effect(EEffectType type, float magnitude, double duration, bool finite = true)
            {
                this.type = type;
                this.magnitude = magnitude;
                endTime = WorldTime.time + duration;
                this.finite = finite;
            }

            public Effect(EEffectType type, float magnitude, string id)
            {
                this.type = type;
                this.magnitude = magnitude;
                endTime = id.GetHashCode();
                finite = false;
            }

            public override bool Equals(object obj)
            {
                return (obj is Effect effect) && (effect == this);
            }

            public override int GetHashCode()
            {
                int result = (int)endTime;
                int xor = (result << (int)type) + (result >> (32 - (int)type));
                return result ^ xor;
            }

            public void StartEffect(EntityLiving entity)
            {
                implementations[(int)type].StartEffect(entity, this);
            }

            public void ContinueEffect(EntityLiving entity)
            {
                implementations[(int)type].ContinueEffect(entity, this);
            }

            public void EndEffect(EntityLiving entity)
            {
                implementations[(int)type].EndEffect(entity, this);
            }
        }


        public interface IEffect
        {
            public void StartEffect(EntityLiving entity, Effect effect);
            public void ContinueEffect(EntityLiving entity, Effect effect);
            public void EndEffect(EntityLiving entity, Effect effect);
        }


        #region Effects Implementation Code



        [System.Serializable]
        public enum EEffectType
        {
            FIRE_RESIT = 0
        }


        private static readonly IEffect[] implementations = new IEffect[] {
            new FireResist(),
        };


        public class FireResist : IEffect
        {
            public void ContinueEffect(EntityLiving entity, Effect effect)
            {
                entity.attributes.damageModifiers.AddModifier(new DamageModInstance(effect.Magnitude, DamageType.fire, effect.MakeID));
            }

            public void EndEffect(EntityLiving entity, Effect effect) {}

            public void StartEffect(EntityLiving entity, Effect effect)
            {
                entity.attributes.damageModifiers.RemoveModifer(effect.MakeID);
            }
        }




        #endregion Effects Implementation Code

    }


}
