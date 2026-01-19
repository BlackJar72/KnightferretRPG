using UnityEngine;
using kfutils.rpg;
using TMPro;
using UnityEngine.EventSystems;
using static rpg.verslika.UICharacter;


namespace rpg.verslika {


    public class ClothingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {        
        [SerializeField] TMP_Text textHolder;
        [SerializeField] GameObject subButtonPanel;
        [SerializeField] GameObject dummyPanel;
        [SerializeField] GameObject subButtonPrefab;
        private Clothing partType;
        private int index;

        public GameObject SubButtonPrefab => subButtonPrefab;


        public void OnPointerEnter(PointerEventData eventData) {
            subButtonPanel.SetActive(true);
            dummyPanel.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData) { 
            if (eventData.fullyExited) 
            {
                subButtonPanel.SetActive(false); 
                dummyPanel.SetActive(false);
            }
        }


        public void Init(int id, Clothing type, string nameBase, ClothingVariant clothing, UICharacter character) 
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
            for(int i = 0; i < clothing.Length; i++)
            {
                ClothingSubButton subButton = Instantiate(subButtonPrefab, subButtonPanel.transform).GetComponent<ClothingSubButton>();
                subButton.Init(id, i, type, character);
            }
            subButtonPanel.SetActive(false);
        }


    }

}