using System;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Basic Action", fileName = "BasicAction", order = 20)]
    public class BasicAction : AbstractAction
    {

        [SerializeField] protected ClipTransition animation;
        [SerializeField] protected AvatarMask avatarMask;


        public override ClipTransition anim => animation;
        public override AvatarMask mask => avatarMask;
        public override int number => 1;

        public override ClipTransition GetSequential(ref int index)
        {
            index = 0;
            return animation;
        }


        public override ClipTransition GetRandom(ref int index)
        {
            return GetSequential(ref index);
        }
        
    }

}
