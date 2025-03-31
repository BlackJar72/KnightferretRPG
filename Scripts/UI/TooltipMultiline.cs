using TMPro;
using UnityEngine;




namespace kfutils.rpg.ui {

    public class TooltipMultiline : TooltipLine {

        [SerializeField] TooltipLine[] children;


        public void SetInfo(params string[] text) {
            if(text != null) {
                base.SetInfo(text[0]);
                int i;
                for(i = 1; (i < text.Length) && (i < children.Length); i++) children[i].SetInfo(text[1]);
                for(; i < children.Length; i++) children[i].SetInfo(null);
            } else {
                gameObject.SetActive(false);
            }
        }

    }


}
