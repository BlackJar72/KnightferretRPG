using TMPro;
using Unity.VisualScripting;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class ItemToolTipUI : MonoBehaviour {

        [SerializeField] TMP_Text itemName;

        [SerializeField] TooltipLine itemType;
        [SerializeField] TooltipLine itemWeight;
        [SerializeField] TooltipLine itemValue;
        [SerializeField] TooltipLine itemDamage;
        [SerializeField] TooltipLine itemSpeed;
        [SerializeField] TooltipMultiline  itemDmgType;
        [SerializeField] TooltipLine itemAP;
        [SerializeField] TooltipLine itemBlock;
        [SerializeField] TooltipLine itemStability;
        [SerializeField] TooltipLine itemParry;
        [SerializeField] TooltipLine itemAR;

        [SerializeField] TMP_Text itemDescription;
        [SerializeField] GameObject finalSpace;


        private delegate void DataSetter(ItemPrototype item);
        private static DataSetter[] dataSetters;



        private void Awake() {
            if(dataSetters == null) {
                dataSetters  = new DataSetter[] { SetFieldsForGeneral, SetFieldsForWeapons, SetFieldsForShield, 
                                                  SetFieldsForArmor, SetFieldsForWearable, SetFieldsForUsable, 
                                                  SetFieldsForConsumable, SetFieldsForWand, SetFieldsForSpecial };
            }
        }


        private void Start()
        {
            gameObject.SetActive(false);    
        }

        // TODO: A system to take item data (or item  description data) and use to configure the text in the tool tip

        // TODO: Code to move the tool-tip to its correct location and make it visible (or spawn it?); also to hide it afterwards


        public void SetDataForItem(ItemPrototype item) {
            // For testing
            SetFieldsForGeneral(item);
            // Real version
            //dataSetters[(int)item.ItemType](item);
        }


        public void ShowToolTip(ItemPrototype item) {
            SetDataForItem(item);
            gameObject.SetActive(true);
        }


        public void HideToolTip() {
            gameObject.SetActive(false);
        }


        private void SetFieldsForGeneral(ItemPrototype item) { 
            itemName.SetText(item.Name);

            itemType.SetInfo("General");
            itemWeight.SetInfo(item.Weight.ToString());
            itemValue.SetInfo(item.Value.GetGoodMoneyString());

            itemDescription.SetText(item.Description);

            itemDamage.SetInfo(null);
            itemSpeed.SetInfo(null);
            itemDmgType.SetInfo(null);
            itemAP.SetInfo(null);
            itemBlock.SetInfo(null);
            itemStability.SetInfo(null);
            itemParry.SetInfo(null);
            itemAR.SetInfo(null);

            finalSpace.gameObject.SetActive(false);
        }


        private void SetFieldsForWeapons(ItemPrototype item) {

        }


        private void SetFieldsForShield(ItemPrototype item) {

        }


        private void SetFieldsForArmor(ItemPrototype item) {

        }


        private void SetFieldsForWearable(ItemPrototype item) {

        }


        private void SetFieldsForUsable(ItemPrototype item) {

        }


        private void SetFieldsForConsumable(ItemPrototype item) {

        }


        private void SetFieldsForWand(ItemPrototype item) {

        }


        private void SetFieldsForSpecial(ItemPrototype item) {

        }


    }

}