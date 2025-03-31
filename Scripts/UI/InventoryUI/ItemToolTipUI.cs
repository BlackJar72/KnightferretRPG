using TMPro;
using Unity.VisualScripting;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class ItemToolTipUI : MonoBehaviour {

        [SerializeField] TMP_Text itemName;

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


        protected void OnEnable() {
            InventoryManager.closeAllInvUI += HideToolTip;
        }


        protected void OnDisable() {
            InventoryManager.closeAllInvUI -= HideToolTip;
        }



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
            dataSetters[(int)item.ItemType](item);
        }


        public void ShowToolTip(ItemPrototype item) {
            SetDataForItem(item);
            gameObject.SetActive(true);
        }


        public void HideToolTip() {
            gameObject.SetActive(false);
        }


        private void SetDescription(string text) {
            itemDescription.SetText(text);
            int w = Mathf.CeilToInt(itemDescription.GetPreferredValues(text)[0]);
            Vector2 rtSize = itemDescription.rectTransform.sizeDelta;
            rtSize[1] = ((w / 354) * 40) + 60;
            itemDescription.rectTransform.sizeDelta = rtSize;
        }


        private void SetFieldsForGeneral(ItemPrototype item) { 
            itemName.SetText(item.Name);

            itemWeight.SetInfo(item.Weight.ToString());
            itemValue.SetInfo(item.Value.GetGoodMoneyString());

            SetDescription(item.Description);

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
            itemName.SetText(item.Name);

            itemWeight.SetInfo(item.Weight.ToString());
            itemValue.SetInfo(item.Value.GetGoodMoneyString());

            SetDescription(item.Description);

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


        private void SetFieldsForShield(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForArmor(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForWearable(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForUsable(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForConsumable(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForWand(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


        private void SetFieldsForSpecial(ItemPrototype item) {
            SetFieldsForGeneral(item);

        }


    }

}