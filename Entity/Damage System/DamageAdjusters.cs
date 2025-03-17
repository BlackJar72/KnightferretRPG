using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace kfutils {

    #region DamageAdjust
    /// <summary>
    /// An enum specifically to allow easy assignment in the unity editor as a type in GUI's like those used by
    /// the RPG Creation Kit (or created with Odin or the like?).  It the realized on the DamageAdjusterList class
    /// to translate these into DamageAdjuster and apply the effect.
    /// </summary>
    public enum DamageAdjustType {
        NONE = 0, // Basic Do Nothing
        // Resistances and Immunities
        FIRE_RESIST = 1,
        FIRE_IMMUNE = 2,
        ELECTRIC_RESIST = 3,
        ELECTRIC_IMMUNE = 4,
        ACID_RESIST = 5,
        ACID_IMMUNE = 6,
        POISON_RESIST = 7,
        POISON_IMMUNE = 8,
        MAGIC_RESIST = 9,
        MAGIC_IMMUNE = 10,
        COLD_RESIST = 11,
        COLD_IMMUNE = 12,
        SPIRITUAL_RESIST = 13,
        SPIRITUAL_IMMUNE = 14,
        // Vulnerabilities
        WEAK_TO_FIRE = 15,
        WEAK_TO_ELECTRIC = 16,
        WEAK_TO_ACID = 17,
        WEAK_TO_POISON = 18,
        WEAK_TO_MAGIC = 19,
        WEAK_TO_COLD = 20,
        WEAK_TO_SPIRITUAL = 21,
        //Misc
        GHOSTLY = 22, // Only harmed by Magical / Spiritual attacks (c.f., "magic to hit" in D&D)
        ANIMATED = 23, // No vital organs, no shock damage (zombies, golems)

    }
    #endregion


    #region DamageAdjuster
    /// <summary>
    /// For adjusting damages in specialized and specific ways, such as immunity to certain damages or requiring a
    /// damage type to harm.
    ///
    /// Note that a general resistance filter, comparing each type and applying a multiplies might be better for some
    /// use cases, such as transient resistances from status effects or protective gear like magic rings.  This is
    /// needed from sistuations when more specific logic is required.
    ///
    /// This is speficically designed to allow for considering combinations -- for example, a ghost might be harmed by
    /// a magical fireball (magical attack) but not mundane fire, as the prsense of magical or spiritual energy allows
    /// for the damage.
    ///
    /// This has been designed for easy entension, though through editing the classes here, rather than throuh
    /// inheritance of some kind of editor magic.
    /// </summary>
    public struct DamageAdjuster {
        public delegate Damages Adjuster(Damages damage);
        public Adjuster adjust;

        public DamageAdjuster(Adjuster adjustment) {
            adjust = adjustment;
        }

        public static Damages None(Damages damage) => damage;

        public static Damages FireImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.fire) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages ElectricImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.electric) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages AcidImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.acid) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages PoisonImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.poison) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages MagicImmune(Damages damages) {
            if(((damages.type & DamageType.magic) > 0) && (((damages.type & DamageType.physical) == 0))) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages ColdImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.cold) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages SpiritualImmune(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.spiritual) {
                damages *= 0;
            }
            return damages;
        }

        public static Damages FireResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.fire) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages ElectricResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.electric) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages AcidResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.acid) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages PoisonResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.poison) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages MagicResist(Damages damages) {
            if(((damages.type & DamageType.magic) > 0) && (((damages.type & DamageType.physical) == 0))) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages ColdResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.cold) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages SpiritualResist(Damages damages) {
            if((damages.type & ~DamageType.magic) == DamageType.spiritual) {
                damages *= 0.5f;
            }
            return damages;
        }

        public static Damages Ghostly(Damages damages) {
            if(((damages.type & (DamageType.magic | DamageType.spiritual)) == 0)) {
                return new Damages(0, 0, damages.type);
            }
            return damages;
        }

        public static Damages Animated(Damages damages) {
            if(((damages.type & DamageType.poison) > 0) && ((damages.type & DamageType.physical) == 0)) {
                return new Damages(0, 0, damages.type);
            }
            damages.shock = 0;
            return damages;
        }

        public static Damages FireWeak(Damages damages) {
            if((damages.type & DamageType.fire) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages ElectricWeak(Damages damages) {
            if((damages.type & DamageType.electric) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages AcidWeak(Damages damages) {
            if((damages.type & DamageType.acid) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages PoisonWeak(Damages damages) {
            if((damages.type & DamageType.poison) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages MagicWeak(Damages damages) {
            if((damages.type & DamageType.magic) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages ColdWeak(Damages damages) {
            if((damages.type & DamageType.cold) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }

        public static Damages SpiritualWeak(Damages damages) {
            if((damages.type & DamageType.spiritual) > 0) {
                damages *= 2.0f;
            }
            return damages;
        }
    }
    #endregion


    #region  DamageAdjustList
    /// <summary>
    /// A class for translating the DamageAjustType enum into a real damage adjusters along with a convenience method
    /// for applying it directly -- thus completing what could be described as my "pseudo-Java-enum pattern."
    /// </summary>
    public class DamageAdjustList {
        //Resistances
        public static readonly DamageAdjuster NONE = new DamageAdjuster(DamageAdjuster.None);
        public static readonly DamageAdjuster FIRE_RESIST = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster FIRE_IMMUNE = new DamageAdjuster(DamageAdjuster.FireImmune);
        public static readonly DamageAdjuster ELECTRIC_RESIST = new DamageAdjuster(DamageAdjuster.ElectricResist);
        public static readonly DamageAdjuster ELECTRIC_IMMUNE = new DamageAdjuster(DamageAdjuster.ElectricImmune);
        public static readonly DamageAdjuster ACID_RESIST = new DamageAdjuster(DamageAdjuster.AcidResist);
        public static readonly DamageAdjuster ACID_IMMUNE = new DamageAdjuster(DamageAdjuster.AcidImmune);
        public static readonly DamageAdjuster POISON_RESIST = new DamageAdjuster(DamageAdjuster.PoisonResist);
        public static readonly DamageAdjuster POISON_IMMUNE = new DamageAdjuster(DamageAdjuster.PoisonImmune);
        public static readonly DamageAdjuster MAGIC_RESIST = new DamageAdjuster(DamageAdjuster.MagicResist);
        public static readonly DamageAdjuster MAGIC_IMMUNE = new DamageAdjuster(DamageAdjuster.MagicImmune);
        public static readonly DamageAdjuster COLD_RESIST = new DamageAdjuster(DamageAdjuster.ColdResist);
        public static readonly DamageAdjuster COLD_IMMUNE = new DamageAdjuster(DamageAdjuster.ColdImmune);
        public static readonly DamageAdjuster SPIRITUAL_RESIST = new DamageAdjuster(DamageAdjuster.SpiritualResist);
        public static readonly DamageAdjuster SPIRITUAL_IMMUNE = new DamageAdjuster(DamageAdjuster.SpiritualImmune);
        // Vulnerabilities
        public static readonly DamageAdjuster WEAK_TO_FIRE = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_ELECTRIC = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_ACID = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_POISON = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_MAGIC = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_COLD = new DamageAdjuster(DamageAdjuster.FireResist);
        public static readonly DamageAdjuster WEAK_TO_SPIRITUAL = new DamageAdjuster(DamageAdjuster.FireResist);
        // Misc
        public static readonly DamageAdjuster GHOSTLY = new DamageAdjuster(DamageAdjuster.Ghostly);
        public static readonly DamageAdjuster ANIMATED = new DamageAdjuster(DamageAdjuster.Animated);


        public static readonly DamageAdjuster[] Adjusters = new DamageAdjuster[]
            {NONE, FIRE_RESIST, FIRE_IMMUNE, ELECTRIC_RESIST, ELECTRIC_IMMUNE, ACID_RESIST, ACID_IMMUNE, POISON_RESIST,
                POISON_IMMUNE, MAGIC_RESIST, MAGIC_IMMUNE, COLD_RESIST, COLD_IMMUNE, SPIRITUAL_RESIST, SPIRITUAL_IMMUNE,
                WEAK_TO_FIRE, WEAK_TO_ELECTRIC, WEAK_TO_ACID, WEAK_TO_POISON, WEAK_TO_POISON, WEAK_TO_COLD,
                WEAK_TO_SPIRITUAL, GHOSTLY, ANIMATED};


        public static DamageAdjuster GetAdjuster(DamageAdjustType type) => Adjusters[(int)type];
        public static Damages Adjust(Damages damage, DamageAdjustType type) => Adjusters[(int)type].adjust(damage);
    }
    #endregion
}