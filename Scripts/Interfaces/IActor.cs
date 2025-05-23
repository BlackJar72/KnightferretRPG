using Animancer;



namespace kfutils.rpg
{

    public interface IActor : IHaveStringID
    {

        public void EquiptItem(ItemStack item);
        public void UnequiptItem(ItemStack item);
        public void UnequiptItem(EEquiptSlot slot);

        public AnimancerLayer ActionLayer { get; }

        public AnimancerState ActionState { get; }

        public void RemoveEquiptAnimation();

        public void GetAimParams(out AimParams aim);

        public void PreSaveEquipt();

    }


}
