using System.Collections;
using TMPro;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class Toast : MonoBehaviour {
        
        [SerializeField] TMP_Text text;
        [SerializeField] float duration = 5.0f;


        private void Start() {
            text.gameObject.SetActive(false);
        }


        IEnumerator Hide(float delay) {
            yield return new WaitForSeconds(delay);
            text.gameObject.SetActive(false);
        }


        public void HideNow() {
            text.gameObject.SetActive(false);
        }


        public void Show(string toast) {
            text.SetText(toast);
            text.gameObject.SetActive(true);
            StartCoroutine(Hide(duration));
        }


        public void Show(string toast, float duration) {
            text.SetText(toast);
            text.gameObject.SetActive(true);
            StartCoroutine(Hide(duration));
        }


    }

}
