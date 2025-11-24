using UnityEngine;



namespace kfutils.rpg.ui {

    public class SimpleBack : MonoBehaviour
    {
        [SerializeField] GameObject from;
        [SerializeField] GameObject to;


        public void GoBack()
        {
            GameManager.Instance.UI.PlayButtonClick();
            from.SetActive(false);
            to.SetActive(to);
        }
    }

}