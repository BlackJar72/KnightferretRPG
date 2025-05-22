using TMPro;
using UnityEngine;


namespace kfutils.rpg
{


    public class LoadButtonUI : SaveButtonUI
    {

        public override void BeClicked()
        {
            saveLoadUI.SelectAsLoad(this);
        }
        


    }


}