using UnityEngine;
using kfutils.rpg;
using TMPro;


namespace rpg.verslika {


    public class AvatarPartButton : MonoBehaviour
    {
        [SerializeField] TMP_Text textHolder;
        private UICharacter.Part partType;
        private int index;
        private UICharacter executor;


        public void Init(int id, UICharacter.Part type, string nameBase, UICharacter character) 
        {
            index = id;
            partType = type;
            if(index <  0)
            {
                textHolder.text = "None"; 
            }
            else
            {
                textHolder.text = nameBase + " #" + index; 
            }
            executor = character;
        }


        public void BeClicked()
        {
            executor.SetPart(partType, index);
        }

    }

}