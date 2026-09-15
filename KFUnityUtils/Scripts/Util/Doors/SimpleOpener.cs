using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils {

    public abstract class SimpleOpener : MonoBehaviour, IDoorOpener {

        public abstract void Activate();


        public abstract void Close();


        public abstract void Open();


    }

}
