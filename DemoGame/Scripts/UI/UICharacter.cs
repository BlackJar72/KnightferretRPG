using UnityEngine;
using System.Globalization;
using kfutils.rpg;
using System;


namespace rpg.verslika {


    public class UICharacter : MonoBehaviour
    {
#region Internal Types
        public enum Part
        {
            hair,
            brows,
            beards
        }


        public enum Clothing
        {
            shirts,
            pants,
            shoes
        }


        [System.Serializable]
        public class PartType
        {
            [SerializeField] string nameBase;
            [SerializeField] int defaultPart;
            [SerializeField] GameObject[] parts;

            [SerializeField] int currentSelection;
            public int CurrentSelection => currentSelection;

            public string NameBase => nameBase;
            public int DefaultPart => defaultPart;
            public GameObject[] Parts => parts;
            public GameObject this[int index] => parts[index]; 
            public int Length => parts.Length;

            public void SetCurrent(int current)
            {
                currentSelection = current;
                parts[currentSelection].SetActive(true);
            }

            public void SetDefault()
            {
                currentSelection = defaultPart;
            }
        }


        [System.Serializable]
        public class ClothingVariant
        {
            [SerializeField] GameObject[] parts;
            
            public int DefaultPart => 0;
            public GameObject[] Parts => parts;
            public GameObject this[int index] => parts[index]; 
            public int Length => parts.Length;
        }


        [System.Serializable]
        public class ClothingType
        {
            [SerializeField] string nameBase;
            [SerializeField] GameObject defaultPart;
            [SerializeField] bool isOptional;
            [SerializeField] ClothingVariant[] parts;

            public ClothingID currentSelection; 

            public string NameBase => nameBase;
            public bool IsOptional => isOptional;
            public GameObject DefaultPart => defaultPart;
            public ClothingVariant[] Parts => parts;
            public ClothingVariant this[int index] => parts[index]; 
            public int Length => parts.Length;
        }
#endregion Internal Types


        [SerializeField] AvatarCreateUI avatarCreateUI;
        [SerializeField] PartType hairs;
        [SerializeField] PartType brows;
        [SerializeField] PartType beards;

        [SerializeField] ClothingType pants;
        [SerializeField] ClothingType shirts;
        [SerializeField] ClothingType shoes;

        public int CurrentHair => hairs.CurrentSelection;
        public int CurrentBrow => brows.CurrentSelection;
        public int CurrentBeard => beards.CurrentSelection;

        public ClothingID CurrentShirt => shirts.currentSelection;
        public ClothingID CurrentPants => pants.currentSelection;
        public ClothingID CurrentShoes => shoes.currentSelection;


        private void Awake()
        {
            hairs.SetDefault();
            brows.SetDefault();
            beards.SetDefault();
        }


        public void NewCharacter()
        {
            hairs.SetDefault();
            brows.SetDefault();
            beards.SetDefault();        
            DefaultClothing();
            DoSetPart(hairs, hairs.CurrentSelection);
            DoSetPart(brows, brows.CurrentSelection);
            DoSetPart(beards, beards.CurrentSelection);
        } 
 

        // Use this on a copy of the reference models to get a source for the character 
        // when loaded.
        public void SetFromHumanBody(HumanBody body, bool isArms) 
        {
            if(isArms)
            {
                LoadSetClothing(Clothing.shirts, body.CurShirt.Item, body.CurShirt.Variant);                
            }
            else 
            {
                LoadSetPart(Part.hair, body.CurHair);
                LoadSetPart(Part.brows, body.CurBrows);
                LoadSetPart(Part.beards, body.CurBeard);
                LoadSetClothing(Clothing.pants, body.CurPants.Item, body.CurPants.Variant);
                LoadSetClothing(Clothing.shirts, body.CurShirt.Item, body.CurShirt.Variant);
                LoadSetClothing(Clothing.shoes, body.CurShoes.Item, body.CurShoes.Variant);
            }
        }


        public void SetPart(Part part, int index)
        {
            switch(part)
            {
                case Part.hair: DoSetPart(hairs, index); break;
                case Part.brows: DoSetPart(brows, index); break;
                case Part.beards: DoSetPart(beards, index); break;
            }
        }


        public void DoSetPart(PartType part, int index)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                if(renderers[i].gameObject.name.Contains(part.NameBase)) renderers[i].gameObject.SetActive(false);
            }
            if((index > -1) && (index < part.Length) && (part[index] != null)) part.SetCurrent(index);
            avatarCreateUI.ApplyColorsUIModel();
        }


        public void LoadSetPart(Part part, int index)
        {
            switch(part)
            {
                case Part.hair: DoLoadSetPart(hairs, index); break;
                case Part.brows: DoLoadSetPart(brows, index); break;
                case Part.beards: DoLoadSetPart(beards, index); break;
            }
        }


        public void DoLoadSetPart(PartType part, int index)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if((renderers == null) || (renderers.Length < 1)) return;
            for(int i = 0; i < renderers.Length; i++)
            {
                if(renderers[i].gameObject.name.Contains(part.NameBase)) renderers[i].gameObject.SetActive(false);
            }
            if((index > -1) && (index < part.Length) && (part[index] != null)) part.SetCurrent(index);
        }


        public GameObject GetPartCopy(Part part, int index, Transform trans)
        {
            return part switch
            {
                Part.hair => DoGetPartCopy(hairs, index, trans),
                Part.brows => DoGetPartCopy(brows, index, trans),
                Part.beards => DoGetPartCopy(beards, index, trans),
                _ => null,
            };
        }


        public GameObject DoGetPartCopy(PartType part, int index, Transform trans)
        {
            return Instantiate(part[index], trans);
        }


        public void MakePartsButtons(Part part, GameObject buttonPrefab, Transform buttonParent)
        {
            PartType partType = part switch
            {
                Part.hair => hairs,
                Part.brows => brows,
                Part.beards => beards,
                _ => hairs,
            };
            if(partType.Length < 1) return;
            if(buttonPrefab.TryGetComponent<AvatarPartButton>(out AvatarPartButton avatarPart)) {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                int num = Mathf.Min(partType.Length, 15);
                for(int i = -1; i < num; i++)
                {
                    GameObject button = Instantiate(buttonPrefab, buttonParent);
                    button.GetComponent<AvatarPartButton>().Init(i, part, textInfo.ToTitleCase(partType.NameBase), this);
                }
            } 
            else return; 
        }


        internal void StartSetClothing(Clothing partType, int index, int subIndex)
        {
            // Passing this on up this conceptual "stack" to include arms (safrely)
            avatarCreateUI.SetClothing(partType, index, subIndex);
        }


        internal void SetClothing(Clothing partType, int index, int subIndex)
        {
            switch(partType)
            {
                case Clothing.pants: DoSetClothing(pants, index, subIndex); return;
                case Clothing.shirts: DoSetClothing(shirts, index, subIndex); return;
                case Clothing.shoes: DoSetClothing(shoes, index, subIndex); return;
                default: return;
            }
        }



        public void DoSetClothing(ClothingType clothing, int index, int subIndex)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                if(renderers[i].gameObject.name.Contains(clothing.NameBase)) renderers[i].gameObject.SetActive(false);
            }
            if((index > -1) && (index < clothing.Length) && (subIndex > -1) 
                        && (subIndex < clothing[index].Length) && (clothing[index][subIndex] != null)) {
                clothing[index][subIndex].SetActive(true);
                clothing.currentSelection.Set(index, subIndex);
            }
            else clothing.DefaultPart.SetActive(true);
            avatarCreateUI.ApplyColorsUIModel();
        }


        internal void DefaultClothing()
        {
            DoLoadSetClothing(pants, -1, -1);
            DoLoadSetClothing(shirts, -1, -1);
            DoLoadSetClothing(shoes, -1, -1);
        }


        internal void LoadSetClothing(Clothing partType, int index, int subIndex)
        {
            switch(partType)
            {
                case Clothing.pants: DoLoadSetClothing(pants, index, subIndex); return;
                case Clothing.shirts: DoLoadSetClothing(shirts, index, subIndex); return;
                case Clothing.shoes: DoLoadSetClothing(shoes, index, subIndex); return;
                default: return;
            }
        }



        public void DoLoadSetClothing(ClothingType clothing, int index, int subIndex)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                if(renderers[i].gameObject.name.Contains(clothing.NameBase)) renderers[i].gameObject.SetActive(false);
            }
            if((index > -1) && (index < clothing.Length) && (subIndex > -1) 
                        && (subIndex < clothing[index].Length) && (clothing[index][subIndex] != null)) {
                clothing[index][subIndex].SetActive(true);
                clothing.currentSelection.Set(index, subIndex);
            }
            else clothing.DefaultPart.SetActive(true);
        }


        internal GameObject GetClothingCopy(Clothing partType, int index, int subIndex, Transform trans)
        {
            return partType switch
            {
                Clothing.pants => DoGetClothingCopy(pants, index, subIndex, trans),
                Clothing.shirts => DoGetClothingCopy(shirts, index, subIndex, trans),
                Clothing.shoes => DoGetClothingCopy(shoes, index, subIndex, trans),
                _ => null,
            };
        }


        internal GameObject DoGetClothingCopy(ClothingType partType, int index, int subIndex, Transform trans)
        {
            return Instantiate(partType[index][subIndex], trans);
        }


        public void MakeClothingButtons(Clothing part, GameObject buttonPrefab, Transform buttonParent)
        {
            ClothingType partType = part switch
            {
                Clothing.pants => pants,
                Clothing.shirts => shirts,
                Clothing.shoes => shoes,
                _ => shirts,
            };
            if(partType.Length < 1) return;
            if(buttonPrefab.TryGetComponent<ClothingButton>(out ClothingButton avatarPart)) {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                int num = Mathf.Min(partType.Length, 15);
                GameObject noneButton = Instantiate(buttonPrefab.GetComponent<ClothingButton>().SubButtonPrefab, buttonParent);
                noneButton.GetComponent<ClothingSubButton>().MakeNone(part, this);
                for(int i = 0; i < num; i++)
                {
                    GameObject button = Instantiate(buttonPrefab, buttonParent);
                    button.GetComponent<ClothingButton>().Init(i, part, textInfo.ToTitleCase(partType.NameBase), partType[i], this);
                }
            } 
            else return; 
        }



    }

}