using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public abstract class AbstractAction : ScriptableObject {

        public abstract ClipTransition anim { get; }
        public abstract AvatarMask mask { get; }
        public abstract int number { get; }
        public abstract ClipTransition GetSequential(int index);
        public abstract ClipTransition GetRandom(int index);

        public abstract AbstractAction Duplicate();


    }

}
