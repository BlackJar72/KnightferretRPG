using TMPro;
using UnityEngine;


namespace kfutils.rpg.ui
{


    public class LoadButtonUI : SaveButtonUI
    {

        public override void BeClicked()
        {
            saveLoadUI.SelectAsLoad(this);
        }
        


    }


}