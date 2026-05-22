using UnityEngine;


namespace kfutils.rpg {

    /// <summary>
    /// A simple struct that can be used to get aim data from a character 
    /// to be used by a weapon or other targetted game object.  Thus, 
    /// logic specific to the character (from the camera for player character, 
    /// from AI for AI controlled character).
    /// </summary>
    public struct AimParams
    {
        /// <summary>
        /// The point from which the aim is directed.
        /// </summary>
        public Vector3 from;
        /// <summary>
        /// A mathematical vector representing the direction 
        /// of the aim. 
        /// </summary>
        public Vector3 toward;
    }

}