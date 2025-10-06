using Animancer;



namespace kfutils.rpg
{

    public interface IActor : IHaveStringID, IMover
    {

        public void EquiptItemToBody(ItemStack item);
        public void UnequiptItemFromBody(ItemStack item);
        public void UnequiptItemFromBody(EEquiptSlot slot);

        public AnimancerLayer ActionLayer { get; }

        public AnimancerState ActionState { get; }

        public CharacterInventory CharInventory { get;  }

        public AnimancerState PlayAction(UnityEngine.AvatarMask mask, ITransition animation, float time = 0);

        public AnimancerState PlayAction(UnityEngine.AvatarMask mask, ITransition animation, System.Action onEnd, float time = 0, float delay = 1.0f);

        public void StopAction();

        public void RemoveEquiptAnimation();

        public void GetAimParams(out AimParams aim);

        public void PreSaveEquipt();

    }


}
