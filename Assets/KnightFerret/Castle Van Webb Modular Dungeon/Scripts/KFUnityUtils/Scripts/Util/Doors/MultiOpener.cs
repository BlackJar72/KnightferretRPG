using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils {

    public class MultiOpener : SimpleOpener {
        [SerializeField] List<SimpleOpener> doors;


        public override void Activate() {
            foreach(SimpleOpener door in doors)
                door.Activate();
        }


        public override void Close() {
            foreach(SimpleOpener door in doors)
                door.Close();
        }


        public override void Open() {
            foreach(SimpleOpener door in doors)
                door.Open();
        }

    }

}
