using TMPro;
using UnityEngine;



namespace kfutils.rpg.ui {

    public class ItemToolTipUI : MonoBehaviour {

        [SerializeField] TMP_Text itemName;

        [SerializeField] TooltipLine itemWeight;
        [SerializeField] TooltipLine itemValue;
        [SerializeField] TooltipLine itemDamage;
        [SerializeField] TooltipLine itemSpeed;
        [SerializeField] TooltipLine castTime;
        [SerializeField] TooltipLine manaCost;
        [SerializeField] TooltipLine range;
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
            InventoryManagement.closeAllInvUI += HideToolTip;
        }


        protected void OnDisable() {
            InventoryManagement.closeAllInvUI -= HideToolTip;
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
            //gameObject.SetActive(true); // Having this here demostrates this is the one in the hierarchy, not dangling somehow
            SetDataForItem(item);
            gameObject.SetActive(true);
        }


        public void HideToolTip() {
            gameObject.SetActive(false);
        }


        private void SetDescription(string text) {
            itemDescription.SetText(System.Environment.NewLine + text);
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

            castTime.SetInfo(null);
            manaCost.SetInfo(null);
            range.SetInfo(null);

            finalSpace.gameObject.SetActive(false);
        }


        private void SetFieldsForWeapons(ItemPrototype item) {
            itemName.SetText(item.Name);

            itemWeight.SetInfo(item.Weight.ToString());
            itemValue.SetInfo(item.Value.GetGoodMoneyString());

            SetDescription(item.Description);

            if (item.EquiptItem is WeaponMelee weapon)
            {
                itemDamage.SetInfo(weapon.GetDamage().ToString());
                itemSpeed.SetInfo((1.0f / weapon.AttackTime).ToString("0.0"));
                itemDmgType.SetInfo(weapon.DamagerSrc.Type.ToString());
                itemAP.SetInfo(((int)(weapon.DamagerSrc.AP * 100)).ToString() + "%");
                itemBlock.SetInfo(((int)(weapon.BlockAmount * 100)).ToString() + "%");
                itemStability.SetInfo(((int)(weapon.Stability * 100)).ToString() + "%");
                itemParry.SetInfo(weapon.ParryWindow.ToString("0.00") + " s");
                itemAR.SetInfo(null);
            }
            else
            {
                itemDamage.SetInfo(null);
                itemSpeed.SetInfo(null);
                itemDmgType.SetInfo(null);
                itemAP.SetInfo(null);
                itemBlock.SetInfo(null);
                itemStability.SetInfo(null);
                itemParry.SetInfo(null);
                itemAR.SetInfo(null);
            }

            castTime.SetInfo(null);
            manaCost.SetInfo(null);
            range.SetInfo(null);

            finalSpace.gameObject.SetActive(false);
        }


        private void SetFieldsForShield(ItemPrototype item) {
            itemName.SetText(item.Name);

            itemWeight.SetInfo(item.Weight.ToString());
            itemValue.SetInfo(item.Value.GetGoodMoneyString());

            SetDescription(item.Description);

            if (item.EquiptItem is ItemShield shield)
            {
                itemDamage.SetInfo(shield.GetDamage().ToString());
                itemSpeed.SetInfo(null);
                itemDmgType.SetInfo(null);
                itemAP.SetInfo(null);
                itemBlock.SetInfo(((int)(shield.BlockAmount * 100)).ToString() + "%");
                itemStability.SetInfo(((int)(shield.Stability * 100)).ToString() + "%");
                if(shield.ParryWindow > 0.0f) itemParry.SetInfo(shield.ParryWindow.ToString("0.00") + " s");
                else itemParry.SetInfo(null);
                itemAR.SetInfo(null);
            }
            else
            {
                itemDamage.SetInfo(null);
                itemSpeed.SetInfo(null);
                itemDmgType.SetInfo(null);
                itemAP.SetInfo(null);
                itemBlock.SetInfo(null);
                itemStability.SetInfo(null);
                itemParry.SetInfo(null);
                itemAR.SetInfo(null);
            }

            castTime.SetInfo(null);
            manaCost.SetInfo(null);
            range.SetInfo(null);

            finalSpace.gameObject.SetActive(false);
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


        public void ShowSpellToolTip(Spell spell) {
            itemName.SetText(spell.Name);

            SetDescription(spell.Description);

            itemWeight.SetInfo(null);
            itemValue.SetInfo(null);

            itemDamage.SetInfo(null);
            itemSpeed.SetInfo(null);
            itemDmgType.SetInfo(null);
            itemAP.SetInfo(null);
            itemBlock.SetInfo(null);
            itemStability.SetInfo(null);
            itemParry.SetInfo(null);
            itemAR.SetInfo(null);

            castTime.SetInfo(spell.CastTime.ToString());
            manaCost.SetInfo(spell.ManaCost.ToString());
            range.SetInfo(GetSpellRangeString(spell));

            finalSpace.gameObject.SetActive(false); 

            gameObject.SetActive(true);
        }


        private string GetSpellRangeString(Spell spell)
        {
            switch(spell.CastType)
            {
                case ESpellTypes.SELF: return "Self";
                case ESpellTypes.TOUCH: return "Touch";
                case ESpellTypes.PROJECTILE: return spell.Range.ToString();
                case ESpellTypes.RANGED: return spell.Range.ToString();
                case ESpellTypes.CONTIUOUS_SELF: return "Self";
                case ESpellTypes.CONTIUOUS_RANGED: return spell.Range.ToString();
                default: return spell.Range.ToString();
            }
        }


    }

}