using UnityEngine;


namespace kfutils.rpg {


    public class VisualEffect : MonoBehaviour
    {
        [SerializeField] GameObject visuals;


        public void Show() => visuals.SetActive(true);
        public void Hide() => visuals.SetActive(false);
        public void Remove() => Destroy(gameObject);
    }

}
