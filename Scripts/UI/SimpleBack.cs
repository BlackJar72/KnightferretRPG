using UnityEngine;



namespace kfutils.rpg.ui {

    public class SimpleBack : MonoBehaviour
    {
        public void GoBack()
        {
            GameManager.Instance.UI.PlayButtonClick();
            GameManager.ReturnFromSpecialCanvas();
        }
    }

}