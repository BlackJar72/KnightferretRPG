using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

      [System.Serializable]
      public class Personality
      {

            public const int MAX_NUM_QUIRKS = 6;
            public const int MAX_TRAIT = 20;
            public const int AVG_TRAIT = 10;
            public const int MIN_TRAIT = 1;
            public const float MAX_SEPARATION = 50.0f; // The maximum distance between personalities (46.5403051129) rounded
            public const float MAX_DIST_INVERSE = 1.0f / MAX_SEPARATION;

            public const float F_MAX_TRAIT = MAX_TRAIT;
            public const float F_AVG_TRAIT = AVG_TRAIT;
            public const float F_MIN_TRAIT = MIN_TRAIT;

            //Core Traits, base on HEXACO
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int open = AVG_TRAIT;
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int moral = AVG_TRAIT; // Honesty-Humility, but dumbed-down for non-psychologists
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int extroverted = AVG_TRAIT;
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int sensitive = AVG_TRAIT;
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int emotional = AVG_TRAIT;
            [Range(MIN_TRAIT, MAX_TRAIT)][SerializeField] int industrious = AVG_TRAIT; // Concientiousness, dumbed-down (though differently than that game that called it neat)

            [Range(MIN_TRAIT, MAX_TRAIT)]public int Open => open;
            [Range(MIN_TRAIT, MAX_TRAIT)]public int Moral => moral; 
            [Range(MIN_TRAIT, MAX_TRAIT)]public int Extroverted => extroverted;
            [Range(MIN_TRAIT, MAX_TRAIT)]public int Sensitive => sensitive;
            [Range(MIN_TRAIT, MAX_TRAIT)]public int Emotional => emotional;
            [Range(MIN_TRAIT, MAX_TRAIT)]public int Industrious => industrious; 


            // Minor traits / Quirks -- refers to small traits based on simple description
            List<MinorTrait> quirks;




            /// <summary>
            /// Returns a compatibility score based on core traits and modeled as the distance between the
            /// two personalities (in an abstract six-dimensional space, of course).
            /// </summary>
            /// <param name="other">The personalit of the one with which compatibilities is being calculated</param>
            /// <returns>A base compatibility between 0.0 (no compatibility) and 1.0 (perfect match) </returns>
            /// TODO: This will need to be tweaked through testing mostllikely the inclusion of a scaling factor
            /// TODO  and/or other addition transformation.
            public float Compatibility(Personality other)
            {
                  // This the equivalent of scaling the distance to 1
                  return (MAX_SEPARATION - Mathf.Sqrt((float)((open - other.open)
                                                * (open - other.open))
                                          + ((moral - other.moral)
                                                * (open - other.moral))
                                          + ((extroverted - other.extroverted)
                                                * (extroverted - other.extroverted))
                                          + ((sensitive - other.sensitive)
                                                * (sensitive - other.sensitive))
                                          + ((emotional - other.emotional)
                                                * (emotional - other.emotional))
                                          + ((industrious - other.industrious)
                                                * (industrious - other.industrious)))) * MAX_DIST_INVERSE;
            }




      }
    

}
