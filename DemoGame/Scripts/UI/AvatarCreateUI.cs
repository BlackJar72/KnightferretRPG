using UnityEngine;
using kfutils.rpg;
using UnityEngine.UI;
using System.Linq;


namespace rpg.verslika {


    public class AvatarCreateUI : MonoBehaviour
    {

        [SerializeField] GameObject maleAvatar;
        [SerializeField] GameObject femaleAvatar;
        [SerializeField] GameObject maleArms;
        [SerializeField] GameObject femaleArms;
        [SerializeField] AvatarSidePanel sidePanel;

        [SerializeField] ColorSet skinColors;
        [SerializeField] ColorSet hairColors;
        [SerializeField] ColorSet eyeColors;

        [SerializeField] AvatarTopButton hairsButton;
        [SerializeField] GameObject maleHairPanel;
        [SerializeField] GameObject femaleHairPanel;

        [SerializeField] Transform pcMeshParent;
        [SerializeField] Transform pcHips;

        [SerializeField] Transform armsMeshParent;
        [SerializeField] Transform armsHips;


        private bool isMale = true;
        private int skinColor;
        private int hairColor;
        private int eyeColor;
        private bool loadedBefore = false;


        private void OnEnable()
        {
            CharacterCreationUI.CharacterCreationEvent += CopyCharacterForGameplay;
            skinColor = skinColors.DefaultEntry;
            hairColor = hairColors.DefaultEntry;
            eyeColor = eyeColors.DefaultEntry;
            if(loadedBefore) 
            {
                maleAvatar.GetComponent<UICharacter>().NewCharacter();
                femaleAvatar.GetComponent<UICharacter>().NewCharacter();
                SetMale(true);  
            }
            else loadedBefore = true;         
        }


        private void OnDisable()
        {
            CharacterCreationUI.CharacterCreationEvent -= CopyCharacterForGameplay;            
        }


        private void Start()
        {
            SetMale(true);
        }


        public void SetMale(bool male)
        {
            isMale = male;
            maleAvatar.SetActive(isMale);
            femaleAvatar.SetActive(!isMale);
            if(isMale) hairsButton.SetChildPanel(maleHairPanel);
            else hairsButton.SetChildPanel(femaleHairPanel);
            UpdateSex();
        }


        public GameObject GetAvatar()
        {
            if(isMale) return maleAvatar;
            else return femaleAvatar;
        }


        public GameObject GetArms()
        {
            if(isMale) return maleArms;
            else return femaleArms;
        }


        public void ShowSkinTones() => sidePanel.PopulateColors(skinColors, AvatarSidePanel.Mode.skinColor);
        public void ShowHairColors() => sidePanel.PopulateColors(hairColors, AvatarSidePanel.Mode.hairColor);
        public void ShowEyeColors() => sidePanel.PopulateColors(eyeColors, AvatarSidePanel.Mode.eyeColor);
        public void ClearSidePanel() => sidePanel.Clear();


        public void SetColor(int color, AvatarSidePanel.Mode mode)
        {
            switch(mode)
            {
                case AvatarSidePanel.Mode.skinColor:
                    skinColor = color;
                    break;
                case AvatarSidePanel.Mode.hairColor:
                    hairColor = color;
                    break;
                case AvatarSidePanel.Mode.eyeColor:
                    eyeColor = color;
                    break;
                default: return;
            }
            ApplyColorsUIModel();
        }


        public void UpdateSex()
        {
            ApplyColorsUIModel();
            sidePanel.RepopulatePartsForSex(GetAvatar());
        }


        public void ApplyColorsUIModel()
        {
            MaterialShifter[] shifters = maleAvatar.GetComponentsInChildren<MaterialShifter>();
            ApplyColorsToModelPieces(shifters);
            shifters = maleArms.GetComponentsInChildren<MaterialShifter>();
            ApplyColorsToModelPieces(shifters);
            shifters = femaleAvatar.GetComponentsInChildren<MaterialShifter>();
            ApplyColorsToModelPieces(shifters);
            shifters = femaleArms.GetComponentsInChildren<MaterialShifter>();
            ApplyColorsToModelPieces(shifters);
        }


        private void ApplyColorsToModelPieces(MaterialShifter[] shifters)
        {
            foreach(MaterialShifter shifter in shifters)
            {
                switch(shifter.ColorType)
                {
                    case ColorManager.ColorType.SKIN: shifter.SetMaterial(skinColor); break;
                    case ColorManager.ColorType.HAIR: shifter.SetMaterial(hairColor); break;
                    case ColorManager.ColorType.EYES: shifter.SetMaterial(eyeColor); break;
                    default: break;
                }
            }
        }


        internal void SetClothing(UICharacter.Clothing partType, int index, int subIndex)
        {
            GetAvatar().GetComponent<UICharacter>().SetClothing(partType, index, subIndex);
            if(partType == UICharacter.Clothing.shirts) 
            {
                GetArms().GetComponent<UICharacter>().SetClothing(partType, index, subIndex);
            }
        }


        public void ShowHairs() => sidePanel.PopulateHairs(GetAvatar());
        public void ShowBrows() => sidePanel.PopulateBrows(GetAvatar());
        public void ShowBeards() => sidePanel.PopulateBeards(maleAvatar);


        public void ShowShirts() => sidePanel.PopulateShirts(GetAvatar());
        public void ShowPants() => sidePanel.PopulatePants(GetAvatar());
        public void ShowShoes() => sidePanel.PopulateShoes(GetAvatar());


        public void CopyCharacterForGameplay()
        {
            TransferSkinnedMeshes(pcMeshParent, pcHips, GetAvatar());
            TransferSkinnedMeshes(armsMeshParent, armsHips, GetArms());
            if(EntityManagement.playerCharacter.TryGetComponent<HumanBody>(out var pcBody))
            {
                pcBody.SetPartsFromCC(GetAvatar().GetComponent<UICharacter>(), isMale, skinColor, hairColor, eyeColor);
            }
            CharacterCreationUI.SetCreationSuccess(true);
        }

        // TODO: Change the first person arms to mach the body (I need to make all the arms, first, though!)
        // TODO: Save and load/apply body data to/from save files! 

        public static void TransferSkinnedMeshes(Transform target, Transform targetHips, GameObject source)
        {
            SkinnedMeshRenderer[] existing = target.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(SkinnedMeshRenderer child in existing)
            {
                Destroy(child.gameObject);
            }
            GameObject dummy = Instantiate(source); // Need a dummy version so the UI version is not altered
            SkinnedMeshRenderer[] skinnedMeshRenderersList = dummy.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer part in skinnedMeshRenderersList)
            {
                if(part.gameObject.activeSelf) {
                    string cachedRootBoneName = part.rootBone.name;
                    Transform[] newBones = new Transform[part.bones.Length];
                    for (var curBone = 0; curBone < part.bones.Length; curBone++)
                        foreach (Transform newBone in targetHips.GetComponentsInChildren<Transform>())
                            if (newBone.name == part.bones[curBone].name)
                            {
                                newBones[curBone] = newBone;
                            }

                    Transform matchingRootBone = GetRootBoneByName(targetHips, cachedRootBoneName);
                    part.rootBone = matchingRootBone != null ? matchingRootBone : targetHips.transform;
                    part.bones = newBones;
                    Transform transform;
                    (transform = part.transform).SetParent(target);
                    transform.localPosition = Vector3.zero;
                }
            }
            Destroy(dummy);
        }


        public static Transform GetRootBoneByName(Transform parentTransform, string name)
        {
            return parentTransform.GetComponentsInChildren<Transform>().FirstOrDefault(transformChild => transformChild.name == name);
        }



        
    }


}