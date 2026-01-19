using System;
using UnityEngine;
using UnityEngine.UI;


namespace kfutils.rpg.ui {
  

    public class ScreenFX : MonoBehaviour {

        public enum ScreenColor {
            clear = 0,
            water = 1
        }


        [SerializeField] Image image;

        [SerializeField] Color[] colors = new Color[Enum.GetValues(typeof(ScreenColor)).Length];

        void Awake() {
            colors[0] = new Color(255, 255, 255, 0);
            SetColor(0);
        }


        public void SetColor(int index) {
            #if UNITY_EDITOR
            if((index >= colors.Length) || (index < 0)) {
                Debug.LogWarning("ScreenFX.SeteColor(int index): Index does not correspond to valid screen color!");
            }
            #endif
            index %= colors.Length;
            image.color = colors[index];
            image.enabled = (index > 0);
        }


        public void SetColor(ScreenColor screenColor) {
            image.color = colors[(int)screenColor];
            image.enabled = ((int)screenColor > 0);
        }


    }


}
