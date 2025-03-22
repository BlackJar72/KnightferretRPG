using System;
using System.Data.Common;
using UnityEngine;


namespace kfutils {

    [System.Serializable]
    [Flags]
    public enum DamageType {
        physical = 0x1 << 0,
        fire = 0x1 << 1,
        electric = 0x1 << 2,
        acid = 0x1 << 3,
        poison = 0x1 << 4,
        magic = 0x1 << 5,
        cold = 0x1 << 6,
        spiritual = 0x1 << 7
    }


    [System.Serializable]
    public struct Damages {
        public int shock;
        public int wound;
        public DamageType type;

        public Damages(int s, int w, DamageType type) {
            shock = s;
            wound = w;
            this.type = type;
        }

        public Damages(int s, int w, int type) {
            shock = s;
            wound = w;
            this.type = (DamageType)type;
        }

        public Damages(int s, int w) {
            shock = s;
            wound = w;
            type = DamageType.physical;
        }

        public static Damages operator *(Damages damage, float number) {
            damage.shock = (int)Mathf.Ceil(damage.shock * number);
            damage.wound = DamageUtils.CalcWounds(damage.shock);
            return damage;
        }

        public static Damages operator *(float number, Damages damage) {
            damage.shock = (int)Mathf.Ceil(damage.shock * number);
            damage.wound = DamageUtils.CalcWounds(damage.shock);
            return damage;
        }

        public void Nullify() {
            shock = wound = 0;
        }

        public override string ToString()
        {
            return " { S = " + shock + "; " + " W = " + wound + "; type = " + type + "} " ;
        }
    }


    public static class DamageUtils {

        public static int RollDamage(int damageRating) {
            int half  = damageRating / 2;
            int half1 = half + 1;
            return half + UnityEngine.Random.Range(0, half1) + UnityEngine.Random.Range(0, half1);
        }


        public static Damages CalcDamage(int damageRating, int armor, DamageType type = DamageType.physical, float AP = 0) {
            float roll = RollDamage(damageRating);
            float damage = ((roll - (armor * 0.5f))) * (1.0f - KFMath.Asymptote(armor / 100f, 0.5f, 0.4f));
            int realDamage = (int)Mathf.Max(1, (damage * (1.0f - AP)) + (roll * AP));
            return new Damages(realDamage, CalcWounds(realDamage), type);
        }


        public static Damages CalcDamageNoArmor(int damageRating, DamageType type = DamageType.physical) {
            int damage = RollDamage(damageRating);
            return new Damages(damage, CalcWounds(damage), type);
        }


        public static Damages CalcFixedDamage(int damage, DamageType type = DamageType.physical) {
            return new Damages(damage, CalcWounds(damage), type);
        }


        public static void AppyDamageToExisting(ref Damages damages, int armor, float ap) {
            float dam = ((damages.shock - (armor * 0.5f))) * (1.0f - KFMath.Asymptote(armor / 100f, 0.5f, 0.4f));
            damages.shock = (int)Mathf.Max(1, (dam * (1.0f - ap)) + (damages.shock * ap));
            damages.wound = CalcWounds(damages.shock);
        }


        public static int CalcWounds(int shock) {
            if (shock > 12)
            {
                return (shock - 10) / 2;
            }
            else if (shock > 5)
            {
                return 1;
            }
            return 0;
        }

    }


#region Damage Adjusters -- for example, for fire-resistant creatures

    public interface IDamageAdjuster {
        Damages Apply(Damages damage);
    }


    public abstract class DamageAdjusterObj : ScriptableObject, IDamageAdjuster {
        public abstract Damages Apply(Damages damage);
    }

#endregion

}
