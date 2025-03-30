using TMPro;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class ItemToolTipUI : MonoBehaviour {

        [SerializeField] TMP_Text itemName;
        [SerializeField] TMP_Text itemType;
        [SerializeField] TMP_Text itemWeight;
        [SerializeField] TMP_Text itemValue;
        [SerializeField] TMP_Text itemDamage;
        [SerializeField] TMP_Text itemSpeed;
        [SerializeField] TMP_Text itemDmgType1;
        [SerializeField] TMP_Text itemDmgType2;
        [SerializeField] TMP_Text itemAP;
        [SerializeField] TMP_Text itemBlock;
        [SerializeField] TMP_Text itemStability;
        [SerializeField] TMP_Text itemParry;
        [SerializeField] TMP_Text itemAR;
        [SerializeField] TMP_Text itemDescription;


        private void Start()
        {
            gameObject.SetActive(false);    
        }

        // TODO: A system to take item data (or item  description data) and use to configure the text in the tool tip

        // TODO: Code to move the tool-tip to its correct location and make it visible (or spawn it?); also to hide it afterwards


    }

}