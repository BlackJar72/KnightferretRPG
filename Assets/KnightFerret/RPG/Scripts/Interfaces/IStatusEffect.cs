using UnityEngine;



namespace kfutils.rpg {

    /// <summary>
    /// An interface representing a specific instance of a status effect applied to 
    /// a certain Living Entity.
    /// 
    /// Effects in world (not attched to a creature of some kind) will be handled 
    /// differently, treated as items in the world (not as status effects).
    /// </summary>
    public interface IStatusEffect {
        
        /// <summary>
        /// An ID that is unique to this status effect, for use in serialization. 
        /// </summary>
        public string ID { get; }
        
        /// <summary>
        /// Apply changes when the effect is first applied.  
        // 
        // For some effects this may not do anything (such as if the effect is purely 
        // applied over time).
        /// </summary>
        /// <param name="effected"></param>
        public void StartEffect(EntityLiving effected);

        /// <summary>
        /// Effects applied each frame (such as changing health do to regen or poison). 
        /// 
        /// For some effects this may not do anything (such as a buff that is applied at 
        /// the beginning and removed at the end).
        /// </summary>
        /// <param name="effected"></param>
        public void EffectEntityForFrame(EntityLiving effected);

        /// <summary>
        /// Applies changes at the end of an effect, usually repreenting removing 
        /// the effets, uhm, effect -- for example, removing a buff or resistance 
        /// which was applied with StartEffect().
        /// 
        /// For some effects this may do nothing, such as for effect applied purely 
        /// over time -- generally, if changes are made in StartEffect they must be undone 
        /// here, while if no effects were applied this will also do nothing.
        /// </summary>
        /// <param name="effected"></param>
        public void EndEffect(EntityLiving effected);

    }

}
