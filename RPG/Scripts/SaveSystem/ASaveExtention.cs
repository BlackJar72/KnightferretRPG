using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public abstract class ASaveExtention : MonoBehaviour
    {
        public abstract void Save(string fileName);
        public abstract void LoadWorld(string fileName);
        public abstract void LoadPlayer(string fileName);
        public abstract void DeleteSave(string fileName);

    }

}
