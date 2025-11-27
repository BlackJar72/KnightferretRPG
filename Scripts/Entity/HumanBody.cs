using UnityEngine;




namespace kfutils.rpg {

    public class HumanBody : MonoBehaviour
    {
        // Data
        [Range(0, 15)] [SerializeField] int skinColor;
        [Range(0, 15)] [SerializeField] int hairColor;
        [Range(0, 15)] [SerializeField] int eyeColor;

        // Body Parts
        [SerializeField] GameObject head;
        [SerializeField] GameObject body;
        [SerializeField] GameObject legs;
        [SerializeField] GameObject hands;
        [SerializeField] GameObject feet;

        // Acessories
        [SerializeField] GameObject hat;

        // TODO: This need to be able to swap-out (add remove) pieces, to allow 
        // for setting / changing outfits.  It also needs the abiltity to set 
        // colors (really, materials) for any parts with a MaterialShifter 
        // component so as to allow the correct colors to be set.  It will 
        // most likely need a way to store data as well. 
  
    }

}

