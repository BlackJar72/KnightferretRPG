using UnityEngine;
using kfutils.rpg;


namespace rpg.verslika {

    [System.Serializable]
    public struct ClothingID
    {
        public static readonly ClothingID Null = new(-1, -1);

        [SerializeField] int item;
        [SerializeField] int variant;

        public int Item { readonly get { return item; } set { item = value ;}}
        public int Variant { readonly get { return variant; } set { variant = value ;}}
        public void Set(int main, int sub) { item = main; variant = sub; } 
        public ClothingID(int main, int sub) { item = main; variant = sub; } 
    }


    public class HumanBody : MonoBehaviour
    {
        [System.Serializable]
        public struct HumanBodyData
        {
            // Data
            public bool isMale;
            [Range(0, 15)] public int skinColor;
            [Range(0, 15)] public int hairColor;
            [Range(0, 15)] public int eyeColor;

            // Default Parts for Specific Character
            // These are the variants used when not covered by equipt items (such as armor)
            public int hair;
            public int brows;
            public int beard;

            public ClothingID shirt;
            public ClothingID pants;
            public ClothingID shoes;

            // Current Parts
            // These are what the character should have right now
            public int curHair; // Default or None (if covered by a helmet or hat)
            public int curBrows; // Should probably always by the default, but just in case
            public int curBeard; // Default or None (if covered by a helmet or mask)
            public ClothingID curHeadgear; // Includes helmets and hats
            public ClothingID curShirt;
            public ClothingID curPants;
            public ClothingID curShoes;
            public ClothingID curGloves; 

            public HumanBodyData(HumanBody other)
            {
                isMale = other.isMale;
                skinColor = other.skinColor;
                hairColor = other.hairColor;
                eyeColor = other.eyeColor;
                hair = other.hair;
                brows = other.brows;
                beard = other.beard;
                shirt = other.shirt;
                pants = other.pants;
                shoes = other.shoes;
                curHair = other.curHair;
                curBrows = other.CurBrows;
                curBeard = other.CurBeard;
                curHeadgear = other.curHeadgear;
                curShirt = other.curShirt;
                curPants = other.curPants;
                curShoes = other.curShoes;
                curGloves = other.curGloves;            
            }
        }

        [SerializeField] Transform bodyParent;
        [SerializeField] Transform bodyHips;
        [SerializeField] Transform armsParent;
        [SerializeField] Transform armsHips;

        // Data
        [SerializeField] bool isMale;
        [Range(0, 15)] [SerializeField] int skinColor;
        [Range(0, 15)] [SerializeField] int hairColor;
        [Range(0, 15)] [SerializeField] int eyeColor;

        // Default Parts for Specific Character
        // These are the variants used when not covered by equipt items (such as armor)
        [SerializeField] int hair;
        [SerializeField] int brows;
        [SerializeField] int beard;

        [SerializeField] ClothingID shirt;
        [SerializeField] ClothingID pants;
        [SerializeField] ClothingID shoes;

        // Current Parts
        // These are what the character should have right now
        [SerializeField] int curHair; // Default or None (if covered by a helmet or hat)
        [SerializeField] int curBrows; // Should probably always by the default, but just in case
        [SerializeField] int curBeard; // Default or None (if covered by a helmet or mask)
        [SerializeField] ClothingID curHeadgear; // Includes helmets and hats
        [SerializeField] ClothingID curShirt;
        [SerializeField] ClothingID curPants;
        [SerializeField] ClothingID curShoes;
        [SerializeField] ClothingID curGloves;

        // GameObject References

        // Accessor Properties
        public int CurHair => curHair; // Default or None (if covered by a helmet or hat)
        public int CurBrows => curBrows; // Should probably always by the default, but just in case
        public int CurBeard => curBeard; // Default or None (if covered by a helmet or mask)
        public ClothingID CurHeadgear => curHeadgear; // Includes helmets and hats
        public ClothingID CurShirt => curShirt;
        public ClothingID CurPants => curPants;
        public ClothingID CurShoes => curShoes;
        public ClothingID CurGloves => curGloves;


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


        public HumanBodyData GetData()
        {
            return new HumanBodyData(this);
        }


        public void SetColorOnPart(MaterialShifter shifter)
        {
            switch(shifter.ColorType)
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
            if(bodyParent != null)
            {
                MaterialShifter[] shifters = bodyParent.GetComponentsInChildren<MaterialShifter>();
                foreach(MaterialShifter shifter in shifters) SetColorOnPart(shifter);
            }
            if(armsParent != null)
            {
                MaterialShifter[] shifters = armsParent.GetComponentsInChildren<MaterialShifter>();
                foreach(MaterialShifter shifter in shifters) SetColorOnPart(shifter);
            }
        }


        public void SetPartsFromCC(UICharacter proto, bool male, int skinColor, int hairColor, int eyeColor)
        {
            isMale = male;
            this.skinColor = skinColor;
            this.hairColor = hairColor;
            this.eyeColor = eyeColor;
            hair = proto.CurrentHair;
            brows = proto.CurrentBrow;
            beard = isMale ? proto.CurrentBeard : -1;
            shirt = proto.CurrentShirt;
            pants = proto.CurrentPants;
            shoes = proto.CurrentShoes;
            SetToLocalDefaults();
        }


        public void SetToLocalDefaults()
        {
            curHair = hair;
            curBrows = brows;
            curBeard = beard;
            curShirt = shirt;
            curPants = pants;
            curShoes = shoes;
            curGloves = ClothingID.Null;
            curHeadgear = ClothingID.Null;
        }


        public void CopyInto(HumanBody other)
        {
            isMale = other.isMale;
            skinColor = other.skinColor;
            hairColor = other.hairColor;
            eyeColor = other.eyeColor;
            hair = other.hair;
            brows = other.brows;
            beard = other.beard;
            shirt = other.shirt;
            pants = other.pants;
            shoes = other.shoes;
            curHair = other.curHair;
            curBrows = other.curBrows;
            curBeard = other.curBeard;
            curHeadgear = other.curHeadgear;
            curShirt = other.curShirt;
            curPants = other.curPants;
            curShoes = other.curShoes;
            curGloves = other.curGloves;            
        }


        public void CopyInto(HumanBodyData other)
        {
            isMale = other.isMale;
            skinColor = other.skinColor;
            hairColor = other.hairColor;
            eyeColor = other.eyeColor;
            hair = other.hair;
            brows = other.brows;
            beard = other.beard;
            shirt = other.shirt;
            pants = other.pants;
            shoes = other.shoes;
            curHair = other.curHair;
            curBrows = other.curBrows;
            curBeard = other.curBeard;
            curHeadgear = other.curHeadgear;
            curShirt = other.curShirt;
            curPants = other.curPants;
            curShoes = other.curShoes;
            curGloves = other.curGloves;            
        }


        public void RestoreOnLoad()
        {
            GameObject dummyBody = ColorManager.Instance.GetAvatarCopy(isMale);
            UICharacter uiCharacter = dummyBody.GetComponent<UICharacter>();
            uiCharacter.SetFromHumanBody(this, false);
            AvatarCreateUI.TransferSkinnedMeshes(bodyParent, bodyHips, dummyBody);
            Destroy(dummyBody);
            SetBodyColors();
        }


        public void RestoreArmsOnLoad()
        {
            GameObject dummyBody = ColorManager.Instance.GetArmsCopy(isMale);
            UICharacter uiCharacter = dummyBody.GetComponent<UICharacter>();
            uiCharacter.SetFromHumanBody(this, true);
            AvatarCreateUI.TransferSkinnedMeshes(armsParent, armsHips, dummyBody);
            Destroy(dummyBody);
            SetBodyColors();
        }



    }

}

