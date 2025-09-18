using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    [CreateAssetMenu(menuName = "KF-RPG/Actions/Movement Set", fileName = "MovementSet", order = 5)]
    public class MovementSet : ScriptableObject {

        [SerializeField] protected MixerTransition2D crouch;
        [SerializeField] protected MixerTransition2D walk;
        [SerializeField] protected MixerTransition2D run;


        [SerializeField] protected ClipTransition jump;
        [SerializeField] protected ClipTransition fall;
        [SerializeField] protected ClipTransition staggered;

        public MixerTransition2D Crouch => crouch;
        public MixerTransition2D Walk => walk;
        public MixerTransition2D Run => run;


        public ClipTransition Jump => jump;
        public ClipTransition Fall => fall;
        public ClipTransition Staggered => staggered;

    }

}
