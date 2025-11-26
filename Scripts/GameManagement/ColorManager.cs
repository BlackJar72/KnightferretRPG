using Unity.VisualScripting;
using UnityEngine;


namespace kfutils.rpg {

    public class ColorManager : MonoBehaviour
    {
        private static ColorManager instance;
        public static ColorManager Instance => instance;

        public enum ColorType
        {
            SKIN, 
            HAIR,
            EYES
        }

        [SerializeField] Material errorMaterial;
        [SerializeField] Material[] skinTones;
        [SerializeField] Material[] hairColors;
        [SerializeField] Material[] eyeColors;


        void Awake()
        {
            if((instance != null) && (instance != this))
            {
                if(instance.gameObject.GetComponent<GameManager>() == null) Destroy(instance.GameObject());
            }
            instance = this;
        }


        public Material GetMaterial(ColorType colorType, int index)
        {
            return colorType switch
            {
                ColorType.SKIN => skinTones[index],
                ColorType.HAIR => hairColors[index],
                ColorType.EYES => eyeColors[index],
                _ => errorMaterial,
            };
        }

    }

}
