using UnityEngine;


namespace kfutils.rpg.ui {
  

    public class LifebarManager : MonoBehaviour {
        [SerializeField] EntityLiving playerCharacter;
        [SerializeField] BarScaler staminaBar;
        [SerializeField] BarScaler shockBar;
        [SerializeField] BarScaler woundBar;
        [SerializeField] BarScaler manaBar;



        void Update() {
            staminaBar.SetBar(playerCharacter.stamina.RelativeStamina);
            shockBar.SetBar(playerCharacter.health.RelativeShock);
            woundBar.SetBar(playerCharacter.health.RelativeWound);
            manaBar.SetBar(playerCharacter.mana.RelativeMana);
        }


        public void ChangeCharacter(EntityLiving character) {
            playerCharacter = character;
            gameObject.SetActive(playerCharacter != null);
        }
 


    }

}
