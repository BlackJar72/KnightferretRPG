using UnityEngine;
using Animancer;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Random Action", fileName = "RandomAction", order = 25)]
    public class RandomAction : AbstractAction {

        [SerializeField] protected ClipTransition[] animations;
        [SerializeField] protected AvatarMask avatarMask;


        public override ClipTransition anim => animations[Random.Range(0, animations.Length)];

        public override AvatarMask mask => avatarMask;

        public override int number => animations.Length;

        // Ignore index and give random since this for raandom animations; for seqence use ActionSequence.
        public override ClipTransition GetSequential(ref int index) {
            return GetRandom(ref index);
        }
        
        
        public override ClipTransition GetRandom(ref int index)
        {
            index = Random.Range(0, animations.Length);
            return animations[index];
        }

    }


}
