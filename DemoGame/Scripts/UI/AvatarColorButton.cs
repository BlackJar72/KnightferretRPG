using UnityEngine;
using kfutils.rpg;
using UnityEngine.UI;
using static rpg.verslika.ColorSet;


namespace rpg.verslika {
    
    [RequireComponent(typeof(Image))]
    public class AvatarColorButton : MonoBehaviour
    {
        private ColorEntry color;
        private AvatarSidePanel avatarSidePanel;
        private int index;


        public void SetColor(ColorEntry colorEntry, int entryIndex, AvatarSidePanel executor)
        {
            color = colorEntry;
            GetComponent<Image>().color = color.Tone;
            avatarSidePanel =  executor;
            index = entryIndex;
        }


        public void ApplyColor()
        {
            avatarSidePanel.SetColor(index);
        }

    }

}
