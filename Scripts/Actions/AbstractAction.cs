using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public abstract class AbstractAction : ScriptableObject {

        [SerializeField] protected ClipTransition animation;
        [SerializeField] protected AvatarMask avatarMask;


        public ClipTransition anim => animation;
        public AvatarMask mask => avatarMask;


    }

}
