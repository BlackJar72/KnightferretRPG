using System.Collections.Generic;
using kfutils.rpg;
using UnityEngine;


namespace kfutils
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


        public struct Effect
        {
            private readonly EEffectType type;
            private readonly float magnitude;
            private readonly double endTime;
            private readonly bool finite;

            public readonly EEffectType Type => type;
            public readonly float Magnitude => magnitude;
            public readonly double EndTime => endTime;
            public readonly bool Finite => finite;
            public readonly bool ShouldEnd => finite && (endTime < WorldTime.time);
            public readonly string MakeID => type + endTime.ToString() + finite;

            public static bool operator ==(Effect a, Effect b) => (a.type == b.type) && (a.finite == b.finite) && (a.endTime == b.endTime);
            public static bool operator !=(Effect a, Effect b) => (a.type != b.type) || (a.finite != b.finite) || (a.endTime != b.endTime);

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
                implementations[(int)type].StartEffect(entity, ref this);
            }

            public void ContinueEffect(EntityLiving entity)
            {
                implementations[(int)type].ContinueEffect(entity, ref this);
            }

            public void EndEffect(EntityLiving entity)
            {
                implementations[(int)type].EndEffect(entity, ref this);
            }
        }


        public interface IEffect
        {
            public void StartEffect(EntityLiving entity, ref Effect effect);
            public void ContinueEffect(EntityLiving entity, ref Effect effect);
            public void EndEffect(EntityLiving entity, ref Effect effect);
        }


        #region Effects Implementation Code


        public enum EEffectType
        {
            FIRE_RESIT = 0
        }


        private static readonly IEffect[] implementations = new IEffect[] {
            new FireResist(),
        };


        public class FireResist : IEffect
        {
            public void ContinueEffect(EntityLiving entity, ref Effect effect)
            {
                entity.attributes.damageModifiers.AddModifier(new DamageModInstance(effect.Magnitude, DamageType.fire, effect.MakeID));
            }

            public void EndEffect(EntityLiving entity, ref Effect effect) {}

            public void StartEffect(EntityLiving entity, ref Effect effect)
            {
                entity.attributes.damageModifiers.RemoveModifer(effect.MakeID);
            }
        }




        #endregion Effects Implementation Code

    }


}
