using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Action Sequence", fileName = "ActionSequence", order = 27)]
    public class ActionSequence : RandomAction {

        public override ClipTransition GetSequential(ref int index) {
            index %= animations.Length;
            return animations[index];
        }

    }

}
