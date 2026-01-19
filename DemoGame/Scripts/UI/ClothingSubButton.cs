using UnityEngine;
using kfutils.rpg;
using TMPro;
using static rpg.verslika.UICharacter;


namespace rpg.verslika {


    public class ClothingSubButton : MonoBehaviour
    {
        [SerializeField] TMP_Text textHolder;
        private Clothing partType;
        private int index;
        private int subIndex;
        private UICharacter executor;


        public void Init(int id, int subID, Clothing type, UICharacter character) 
        {
            index = id;
            subIndex = subID;
            partType = type;
            executor = character;
            textHolder.text = "Color " + (char)(subIndex + 65); 
        }


        public void MakeNone(Clothing type, UICharacter character) 
        {
            index = -1;
            subIndex = -1;
            partType = type;
            executor = character;
            textHolder.text = "None"; 
        }


        public void BeClicked()
        {
            executor.StartSetClothing(partType, index, subIndex);
        }


    }

}
