using UnityEngine;
using kfutils.rpg;


namespace rpg.verslika {


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

        [SerializeField] UICharacter maleRefPrefab;
        [SerializeField] UICharacter femaleRefPrefab;

        [SerializeField] UICharacter maleRefArmsPrefab;
        [SerializeField] UICharacter femaleRefArmsPrefab;


        void Awake()
        {
            if((instance != null) && (instance != this))
            {
                if(instance.gameObject.GetComponent<GameManager>() == null) Destroy(instance.gameObject);
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


        public GameObject GetHumanPart(UICharacter.Part part, int index, bool isMale, Transform trans)
        {
            if(isMale) return maleRefPrefab.GetPartCopy(part, index, trans);
            else return femaleRefPrefab.GetPartCopy(part, index, trans);
        }


        public GameObject GetHumanClothing(UICharacter.Clothing part, int index, int subIndex, bool isMale, Transform trans)
        {
            if(isMale) return maleRefPrefab.GetClothingCopy(part, index, subIndex, trans);
            else return femaleRefPrefab.GetClothingCopy(part, index, subIndex, trans);
        }


        public GameObject GetAvatarPrefab(bool isMale)
        {
            if(isMale) return maleRefPrefab.gameObject;
            else return femaleRefPrefab.gameObject;            
        }


        public GameObject GetArmsPrefab(bool isMale, Transform trans = null)
        {
            if(isMale) return maleRefArmsPrefab.gameObject;
            else return femaleRefArmsPrefab.gameObject;            
        }


        public GameObject GetAvatarCopy(bool isMale, Transform trans = null)
        {
            if(trans == null) trans = transform.root.transform;
            if(isMale) return Instantiate(maleRefPrefab.gameObject, trans);
            else return Instantiate(femaleRefPrefab.gameObject, trans);            
        }


        public GameObject GetArmsCopy(bool isMale, Transform trans = null)
        {
            if(trans == null) trans = transform.root.transform;
            if(isMale) return Instantiate(maleRefArmsPrefab.gameObject, trans);
            else return Instantiate(femaleRefArmsPrefab.gameObject, trans);            
        }



    }

}
