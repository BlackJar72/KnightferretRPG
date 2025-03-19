using System;
using System.Text;
using UnityEngine;


namespace kfutils.rpg
{

    

     public class Money
    {
        public enum MoneyType {
            Copper = 1,
            Silver = 10,
            Electrum = 50,
            Gold = 100,
            Platinum = 1000
        }


        private static readonly string COMA = ", ";
        private static readonly string NEW_LINE = Environment.NewLine;


        public static MoneyType standard = MoneyType.Gold;
        private static float conversionRateOut = (float)Money.standard / (float)MoneyType.Copper;
        private static float conversionRateIn = (float)Money.standard;

        public static void SetStandard(MoneyType standard) {
            Money.standard = standard;
            conversionRateIn = (float)standard;
            Money.conversionRateOut = (float)Money.standard / (float)MoneyType.Copper;
        }
    

        [SerializeField] int copper;



        public int Copper { get => copper; }
        public float AsSilver { get => (float)copper / (float)MoneyType.Silver; }
        public float AsElectrum { get => (float)copper / (float)MoneyType.Electrum; }
        public float AsGold { get => (float)copper / (float)MoneyType.Gold; }
        public float AsPlatinum { get => (float)copper / (float)MoneyType.Platinum; }



        public void AddCopper(int copper)
        {
            this.copper += copper;
        }


        public bool HasEnoughCopper(int copper) {
            return copper >= this.copper;
        }


        /// <summary>
        /// Used to add or remove money (remember, adding a negative is subtracting, so there is no need for a remove money method).
        /// </summary>
        /// <param name="amount"></param>
        public void AddMoney(float amount)
        {
            copper = Mathf.Max(Mathf.RoundToInt((amount * conversionRateIn)), 0);
        }


        [Obsolete] // Keeping this for now, in case I change my mind
        public bool HasEnough(float amount) {
            return Mathf.CeilToInt(amount * conversionRateIn) >= copper;
        }


        [Obsolete] // Keeping this for now, in case I change my mind
        public bool SpendMoney(float amount) {
            int copperToSpend = Mathf.CeilToInt(amount * conversionRateIn);
            bool result = HasEnough(copperToSpend);
            if(result) {
                copper -= copperToSpend;                
            }
            return result;
        }


        public bool SpendMoneyCopper(int amount) {
            bool result = HasEnoughCopper(amount);
            if(result) {
                copper -= amount;                
            }
            return result;
        }


        public float GetMoneyAsFloat()
        {
            return (float)copper * conversionRateOut;
        }


        public string GetSimpleMoneyString() => GetSimpleMoneyString(copper);


        public static string GetSimpleMoneyString(int copper) {
            return (copper / (int)(standard)) + "." + (copper % (int)standard);
        }


        public string GetComplexMoneyString(bool multiline = false) => GetComplexMoneyString(copper, multiline);


        public static string GetComplexMoneyString(int copper, bool multiline = false) {
            string nextEntry;
            if(multiline) nextEntry = NEW_LINE;
            else nextEntry = COMA;
            StringBuilder builder = new StringBuilder();
            switch(standard) {
                case MoneyType.Platinum:
                    builder.Append(copper / (int)MoneyType.Platinum);
                    builder.Append(" ");
                    builder.Append("Platinum");
                    copper %= (int)MoneyType.Platinum;
                    if(copper > 0) {
                        builder.Append(nextEntry);
                        goto case MoneyType.Gold; 
                    }
                    break;
                case MoneyType.Gold:
                    builder.Append(copper / (int)MoneyType.Gold);
                    builder.Append(" ");
                    builder.Append("Gold");
                    copper %= (int)MoneyType.Gold;
                    if(copper > 0) {
                        builder.Append(nextEntry);
                        goto case MoneyType.Silver; // Unless set as standard, electrum is treated as special; goto silver
                    }
                    break;
                case MoneyType.Electrum:
                    builder.Append(copper / (int)MoneyType.Electrum);
                    builder.Append(" ");
                    builder.Append("Electrum");
                    copper %= (int)MoneyType.Electrum;
                    if(copper > 0) {
                        builder.Append(nextEntry);
                        goto case MoneyType.Silver; 
                    }
                    break;
                case MoneyType.Silver:
                    builder.Append(copper / (int)MoneyType.Silver);
                    builder.Append(" ");
                    builder.Append("Silver");
                    copper %= (int)MoneyType.Silver;
                    if(copper > 0) {
                        builder.Append(nextEntry);
                        goto case MoneyType.Copper; 
                    }
                    break;
                case MoneyType.Copper:
                    builder.Append(copper);
                    builder.Append(" ");
                    builder.Append("Copper");
                    break;
                default: break;
            }
            return builder.ToString();
        }






    }


}
