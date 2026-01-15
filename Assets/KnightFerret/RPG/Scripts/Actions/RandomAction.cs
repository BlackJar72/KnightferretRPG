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
        public override ClipTransition GetSequential(int index) {
            return GetRandom(index);
        }


        public override AbstractAction Duplicate()
        {
            RandomAction output = Instantiate(this);
            output.animations = new ClipTransition[animations.Length];
            for (int i = 0; i < animations.Length; i++) output.animations[i] = animations[i].Clone();
            return output;
        }
        
        
        public override ClipTransition GetRandom(int index)
        {
            index = Random.Range(0, animations.Length);
            return animations[index];
        }

    }


}
