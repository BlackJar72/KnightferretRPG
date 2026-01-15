using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Action Sequence", fileName = "ActionSequence", order = 27)]
    public class ActionSequence : RandomAction {

        public override ClipTransition GetSequential(int index) {
            index %= animations.Length;
            return animations[index];
        }


        public override AbstractAction Duplicate()
        {
            ActionSequence output = Instantiate(this);
            output.animations = new ClipTransition[animations.Length];
            for (int i = 0; i < animations.Length; i++) output.animations[i] = animations[i].Clone();
            return output;
        }

    }

}
