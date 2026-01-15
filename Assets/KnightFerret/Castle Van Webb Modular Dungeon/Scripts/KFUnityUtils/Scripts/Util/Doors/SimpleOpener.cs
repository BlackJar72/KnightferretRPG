using UnityEngine;


namespace kfutils {

    public abstract class SimpleOpener : MonoBehaviour, IDoorOpener {

        public abstract void Activate();


        public abstract void Close();


        public abstract void Open();


        internal void SetStateOpen(bool isOpen)
        {
            if(isOpen) Open();
            else Close();
        }
    }

}
