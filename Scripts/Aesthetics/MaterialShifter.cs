using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    public class MaterialShifter : MonoBehaviour
    {
        [SerializeField] int matToChange;
        [SerializeField] Renderer theRenderer;
        [SerializeField] ColorManager.ColorType colorType;

        private List<Material> mats;


        private void Awake()
        {            
            mats = new(theRenderer.materials);
        }


        public void SetMaterial(Material mat)
        {
            mats[matToChange] = mat;
            theRenderer.SetMaterials(mats);
        }


        public void SetMaterial(int which)
        {
            SetMaterial(ColorManager.Instance.GetMaterial(colorType, which));
        }


    }

}
