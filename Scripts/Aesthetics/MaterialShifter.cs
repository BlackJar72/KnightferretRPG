using System.Collections.Generic;
using UnityEngine;



namespace kfutils.rpg {

    public class MaterialShifter : MonoBehaviour
    {
        [SerializeField] int matToChange;
        [SerializeField] ColorManager.ColorType colorType;

        private List<Material> mats;
        private Renderer theRenderer;

        public ColorManager.ColorType ColorType => colorType;


        // int current = 0;
        // float timer = 1.0f;
        // public void Update()
        // {
        //     if(timer < Time.time)
        //     {
        //         timer = (timer + 1.0f);
        //         SetMaterial(current);
        //         current = (++current) % 16;
        //     }
        // }


        private void Awake()
        {   
            theRenderer = GetComponent<Renderer>();         
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
