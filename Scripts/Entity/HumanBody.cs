using UnityEngine;




namespace kfutils.rpg {

    public class HumanBody : MonoBehaviour
    {
        [SerializeField] BodyType bodyOptions;

        // Data
        [Range(0, 15)] [SerializeField] int skinColor;
        [Range(0, 15)] [SerializeField] int hairColor;
        [Range(0, 15)] [SerializeField] int eyeColor;

        // Main Body Parts
        [SerializeField] GameObject head;
        [SerializeField] GameObject body;
        [SerializeField] GameObject legs;
        [SerializeField] GameObject handr;
        [SerializeField] GameObject handl;
        [SerializeField] GameObject feet;

        // Other parts
        [SerializeField] GameObject eyes;
        [SerializeField] GameObject hair;
        [SerializeField] GameObject brows;
        [SerializeField] GameObject beard;

        // Acessories
        [SerializeField] GameObject hat;

        // TODO: This need to be able to swap-out (add remove) pieces, to allow 
        // for setting / changing outfits.  It also needs the abiltity to set 
        // colors (really, materials) for any parts with a MaterialShifter 
        // component so as to allow the correct colors to be set.  It will 
        // most likely need a way to store data as well. 


        // For testing, may later be removed.
        private void Start()
        {
            SetBodyColors();
        }


        public void SetColorOnPart(GameObject part)
        {
            if(part == null) return;
            if(part.TryGetComponent(out MaterialShifter shifter)) switch(shifter.ColorType)
            {
                case ColorManager.ColorType.SKIN:
                    shifter.SetMaterial(skinColor);
                    return;
                case ColorManager.ColorType.HAIR:  
                    shifter.SetMaterial(hairColor);
                    return; 
                case ColorManager.ColorType.EYES:  
                    shifter.SetMaterial(eyeColor);
                    return;  
            }
        }


        [ContextMenu("Set Colors")]
        public void SetBodyColors()
        {
            SetColorOnPart(head);
            SetColorOnPart(body);
            SetColorOnPart(legs);
            SetColorOnPart(handr);
            SetColorOnPart(handl);
            SetColorOnPart(feet); 
            SetColorOnPart(eyes);
            SetColorOnPart(hair);
            SetColorOnPart(brows);
            SetColorOnPart(beard);
            // Extra
            SetColorOnPart(hat);
        }



    }

}

