using UnityEngine;


namespace kfutils.rpg
{

    public interface INotifiableT<T> 
    {
        public void BeNotified(T notifier);
    }


}