using System;
using TMPro;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class TooltipLine : MonoBehaviour {

        [SerializeField] TMP_Text info;


        public void SetInfo(string text) {
            if(string.IsNullOrEmpty(text)) {
                gameObject.SetActive(false);
            } else {
                info.SetText(text);
                gameObject.SetActive(true);
            }
        }

    }

}
