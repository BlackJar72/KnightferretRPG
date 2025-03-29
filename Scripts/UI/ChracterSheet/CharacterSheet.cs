using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace kfutils.rpg.ui {

    public class CharacterSheet : MonoBehaviour {

        // First Pane
        [SerializeField] PCTalking pc;

        [SerializeField] TMP_Text pcName;

        [SerializeField] TMP_Text levelTxt;
        [SerializeField] TMP_Text xpTxt;

        [SerializeField] TMP_Text strTxt;
        [SerializeField] TMP_Text agilTxt;
        [SerializeField] TMP_Text vitTxt;
        [SerializeField] TMP_Text endTxt;
        [SerializeField] TMP_Text intTxt;
        [SerializeField] TMP_Text chrTxt;
        [SerializeField] TMP_Text sprTxt;

        // Second Pane
        [SerializeField] TMP_Text healthTxt;
        [SerializeField] TMP_Text staminaTxt;
        [SerializeField] TMP_Text manaTxt;

        [SerializeField] TMP_Text speedTxt;
        [SerializeField] TMP_Text encTxt;
        [SerializeField] TMP_Text jumpTxt;
        [SerializeField] TMP_Text defenceTxt;
        [SerializeField] TMP_Text attackTxt;

        [SerializeField] TMP_Text damageTxt;
        [SerializeField] TMP_Text arTxt;




        void Start() {
            if(pc == null) pc = EntityManagement.playerCharacter;
            Redraw();
        }


        void OnEnable() {
            if(pc != null) Redraw();
        }


        void OnDisable() {}


        public void Redraw() {
            pcName.SetText(pc.GetPersonalName());

            levelTxt.SetText("Level: " + pc.attributes.level);
            xpTxt.SetText("Level: " + "TODO!!!");

            strTxt.SetText("Strength: " + pc.attributes.baseStats.Strength);
            agilTxt.SetText("Agility: " + pc.attributes.baseStats.Agility);
            vitTxt.SetText("Vitality: " + pc.attributes.baseStats.Vitality);
            endTxt.SetText("Endurance: " + pc.attributes.baseStats.Endurance);
            intTxt.SetText("Intelligence: " + pc.attributes.baseStats.Intelligence);
            chrTxt.SetText("Charisma: " + pc.attributes.baseStats.Charisma);
            sprTxt.SetText("Spirit: " + pc.attributes.baseStats.Spirit);

            // Second Pane
            healthTxt.SetText("Health: " + pc.health.BaseHealth);
            staminaTxt.SetText("Stamina: " + pc.stamina.baseStamina);;
            manaTxt.SetText("Health: " + pc.mana.baseMana);

            speedTxt.SetText("Speed: " + pc.attributes.crouchSpeed + "/ " + pc.attributes.walkSpeed + " / " + pc.attributes.runSpeed);
            encTxt.SetText("Carry Weight: " + pc.attributes.halfEncumbrance);
            jumpTxt.SetText("Jump Force: " + pc.attributes.jumpForce);
            defenceTxt.SetText("Defence Bonus: " + pc.attributes.naturalArmor);
            attackTxt.SetText("Melee Bonus: " + pc.attributes.meleeDamageBonus);

            damageTxt.SetText("Attack Dmg: " + "TODO!!!");
            arTxt.SetText("Armor Rating: " + "TODO!!!");
        }

    }

}
