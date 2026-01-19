using System.Collections.Generic;
using UnityEngine;
using kfutils.rpg;


namespace rpg.verslika {


    public class MaterialShifter : MonoBehaviour
    {
        [SerializeField] int matToChange;
        [SerializeField] ColorManager.ColorType colorType;

        private List<Material> mats;
        private Renderer theRenderer;

        public ColorManager.ColorType ColorType => colorType;


        private void Awake()
        {  
            theRenderer = GetComponent<Renderer>();         
            mats = new(theRenderer.materials);
        }


        private void SetMaterial(Material mat)
        { 
#if UNITY_EDITOR
            try {
                mats[matToChange] = mat;            
                theRenderer.SetMaterials(mats);
            } 
            catch (System.Exception e)
            {
                if(mats == null)
                {
                    Debug.LogError("Field \"mats\" has not been properly intialized for " + gameObject.name 
                        + " in " + gameObject.transform.parent.gameObject.name + ".");
                    return;
                } else if(matToChange >= mats.Count) {
                    Debug.LogError(gameObject.name + " in " + gameObject.transform.parent.gameObject.name 
                        + " is set for material #" + matToChange + " but there are only " + mats.Count + " materials. ");    
                }
                if(mat == null)
                {
                    Debug.LogError("Parameter \"mat\" was null for " + gameObject.name 
                        + " in " + gameObject.transform.parent.gameObject.name + ".");
                    return;
                }
                Debug.LogError(e);
            }
#endif
            mats[matToChange] = mat;            
            theRenderer.SetMaterials(mats);
        }


        public void SetMaterial(int which)
        {
            SetMaterial(ColorManager.Instance.GetMaterial(colorType, which));
        }


    }

}
