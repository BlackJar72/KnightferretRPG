using System;
using UnityEngine;
using System.Collections.Generic;


namespace kfutils {

    /// This is a second system of damage adjustment intendended to rpresent temporary (or, more broadly, non-inherent)
    /// resistances and vulnerabilities, such as those granted by spells, potions, or gear.
    [Serializable]
    public class DamageModifiers : IDamageAdjuster {

        [SerializeField] float[] resists    = new float[8];
        [SerializeField] float[] weaknesses = new float[8];

        [SerializeField] List<DamageModInstance> modifiers = new List<DamageModInstance>();


        public DamageModifiers Copy() {
            DamageModifiers copy = new();
            Array.Copy(resists, 0, copy.resists, 0, copy.resists.Length);
            Array.Copy(weaknesses, 0, copy.weaknesses, 0, copy.weaknesses.Length);
            for(int i = 0; i < modifiers.Count; i++) copy.modifiers.Add(modifiers[i]);
            return copy;
        }


        public Damages Apply(kfutils.Damages damage) {
            float resist = 0f;
            float weakness = 0f;
            float result;

            if((damage.type & DamageType.physical) > 0) {
                resist = resists[0];
                weakness = weaknesses[0];
            } else {
                for(int i = 1; i < 8; i++) {
                    if(((int)damage.type & (0x1 << i)) > 0) {
                        resist   = Mathf.Max(resists[i],    resist);
                        weakness = Mathf.Max(weaknesses[i], weakness);
                    }
                }
            }
            result = Mathf.Max(1.0f + weakness - resist, 0f);
            return damage * result;
        }


        public float Apply(float damage, DamageType type) {
            float resist = 0f;
            float weakness = 0f;
            float result;

            if((type & DamageType.physical) > 0) {
                resist = resists[0];
                weakness = weaknesses[0];
            } else {
                for(int i = 1; i < 8; i++) {
                    if(((int)type & (0x1 << i)) > 0) {
                        resist   = Mathf.Max(resists[i],    resist);
                        weakness = Mathf.Max(weaknesses[i], weakness);
                    }
                }
            }
            result = Mathf.Max(1.0f + weakness - resist, 0f);
            return damage * result;
        }


        /// <summary>
        /// Create the modifiers; for efficiency this is called when the modifiers in effect change, not when applied to
        /// actual damage (which is likely to happen more frequently, especially with long-term modifiers such as those
        /// from worn equipment).
        /// </summary>
        public void BuildFromModifiers() {
            ClearAll();
            foreach(DamageModInstance mod in modifiers) {
                if(mod.Amount > 0) {
                    AddVulnerability(mod.Amount, mod.Type);
                } else {
                    AddResistance(-mod.Amount, mod.Type);
                }
            }
        }


        /// <summary>
        /// Add a modifier; it will only add it if not already present, so as to avoid having the same modifier in
        /// the list more than once (which shouldn't really happen).  This does not sreeen out multiple effects of
        /// the same type, but should prevent more than one copy exact same effect.
        /// </summary>
        /// <param name="modifier"></param>
        public void AddModifier(DamageModInstance modifier) {
            // Only one instance should be added of any given modifier
            if(!modifiers.Contains(modifier)) {
                modifiers.Add(modifier);
            }
            BuildFromModifiers();
        }


        /// <summary>
        /// Removers a specific modifier from the list
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifer(DamageModInstance modifier) {
            modifiers.Remove(modifier);
            BuildFromModifiers();
        }


        /// <summary>
        /// Removers a specific modifier from the list
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifer(string id)
        {
            RemoveModifer((long)id.GetHashCode());
        }


        /// <summary>
        /// Removers a specific modifier from the list
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifer(long id) {
            long idHash = (long)id.GetHashCode();
            for(int i = modifiers.Count - 1; i > -1; i--)
            {
                if(modifiers[i].ID == id)
                {
                    modifiers.RemoveAt(i);
                    break;
                }
            }
            BuildFromModifiers();
        }


        /// <summary>
        /// Removes all modifiers; primarily intended for testing purposes.
        /// </summary>
        public void ClearModifiers() {
            modifiers.Clear();
            ClearAll();
        }


        protected void AddResistance(float amount, DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) resists[i] = Mathf.Max(resists[i], amount);
            }
        }


        protected void SetResistance(float amount, DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) resists[i] = amount;
            }
        }


        protected void ClearResistance(DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) resists[i] = 0;
            }
        }


        protected void AddVulnerability(float amount, DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) weaknesses[i] = Mathf.Max(weaknesses[i], amount);
            }
        }


        protected void SetVulnerability(float amount, DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) weaknesses[i] = amount;
            }
        }


        protected void ClearVulnerability(DamageType type) {
            for(int i = 0; i < 8; i++) {
                if(((int)type & (0x1 << i)) > 0) weaknesses[i] = 0;
            }
        }


        protected void ClearAll() {
            for(int i = 0; i < 8; i++) {
                resists[i] = weaknesses[i] = 0;
            }
        }


    }


    /// <summary>
    /// A container for specific adjustments, that is specific applications of a modifier that can be added to list and
    /// later removed to keep track of what effects should currently be applied.
    /// </summary>
    [Serializable]
    public struct DamageModInstance {
        private readonly float amount; // Positive -> vulnerability, negative -> resistance
        private readonly DamageType type;
        private long id;
        public float Amount => amount;
        public DamageType Type => type;
        public long ID => id;

        public DamageModInstance(float amount, DamageType type, string id) {
            this.amount = amount;
            this.type = type;
            this.id = (long)id.GetHashCode();
        }

        public DamageModInstance(float amount, DamageType type, long id) {
            this.amount = amount;
            this.type = type;
            this.id = id;
        }

        public void SetID(long newId) {
            id = newId;
        }

        public static bool operator ==(DamageModInstance a, DamageModInstance b) => a.id == b.id;
        public static bool operator !=(DamageModInstance a, DamageModInstance b) => a.id != b.id;

        public static bool operator ==(DamageModInstance a, string b) => a.id == (long)b.GetHashCode();
        public static bool operator !=(DamageModInstance a, string b) => !(a.id == (long)b.GetHashCode());
        public static bool operator ==(string b, DamageModInstance a) => a.id == (long)b.GetHashCode();
        public static bool operator !=(string b, DamageModInstance a) => !(a.id == (long)b.GetHashCode());

        public static bool operator ==(DamageModInstance a, long b) => a.id == b;
        public static bool operator !=(DamageModInstance a, long b) => !(a.id == b);
        public static bool operator ==(long b, DamageModInstance a) => a.id == b;
        public static bool operator !=(long b, DamageModInstance a) => !(a.id == b);

        public static bool operator ==(DamageModInstance a, int b) => a.id == b;
        public static bool operator !=(DamageModInstance a, int b) => !(a.id == b);
        public static bool operator ==(int b, DamageModInstance a) => a.id == b;
        public static bool operator !=(int b, DamageModInstance a) => !(a.id == b);


        #nullable enable
        public override bool Equals(object? other) {
            if(other is DamageModInstance) return ((DamageModInstance)other).id == id;
            else if(other is string) return ((long)other.GetHashCode()) == id;
            else if((other is long) || (other is int)) return id == (long)other;
            else return false;
        }
        #nullable disable
        public override int GetHashCode() => (int)id;
        public override string ToString() => "[Damage Mode #: " + id + "; Amount = " + amount + ", type = " + type + "]";
    }


    [Serializable]
    public class DamageModSource  {
        [SerializeField] float amount = 0f;
        [SerializeField] DamageType type;
        [SerializeField] string id;
        [SerializeField] bool unique;
        public float Amount => amount;
        public DamageType Type => type;
        public string ID => id;
        public bool Equals(DamageModSource other) => id.Equals(other.id);
        #nullable enable
        public override bool Equals(object? other) => (other is DamageModSource) && id.Equals(((DamageModSource)other).id);
        #nullable disable
        public static bool operator ==(DamageModSource a, DamageModSource b) => a.id.Equals(b.id);
        public static bool operator !=(DamageModSource a, DamageModSource b) => !a.id.Equals(b.id);
        public override int GetHashCode() => id.GetHashCode();

        public DamageModSource(float amount, DamageType type, string id, bool unique = false) {
            this.amount = amount;
            this.type = type;
            this.id = id;
            this. unique = unique;
        }

        public DamageModInstance CreateModifier() {
            if(unique) {
                // for persistent effects from (for example) and item, where the ID is from and unique to the item (etc.)
                return new DamageModInstance(amount, type, id.GetHashCode());
            } else {
                // for effects from generic sources, such as spells or potions, which may be created more than once or
                // from different sources
                return new DamageModInstance(amount, type, (uint)id.GetHashCode() | (((long)Time.time) << 32));
            }
        }
    }


}
