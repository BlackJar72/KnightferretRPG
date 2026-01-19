using UnityEngine;
using kfutils.rpg;
using UnityEngine.UI;


namespace rpg.verslika {


    public class AvatarSidePanel : MonoBehaviour
    {
        public enum Mode
        {
            skinColor,
            hairColor,
            eyeColor,
            hairs,
            brows,
            beards,
            shirts,
            pants,
            shoes,
            gloves
        }

        [SerializeField] AvatarCreateUI avatarCreateUI;
        [SerializeField] GameObject colorButtonPrefab;
        [SerializeField] GameObject partsButtonPrefab; 
        [SerializeField] GameObject clothsButtonPrefab; 

        private Mode mode;


        private void Start()
        {
            CharacterCreationUI.BackToStartEvent += Clear;
            CharacterCreationUI.NewCharacterEvent += Clear;            
        }


        private void OnEnable()
        {
            Clear();
        }


        private void OnDisable()
        {
            Clear();
        }


        private void OnDestroy()
        {
            CharacterCreationUI.BackToStartEvent -= Clear;
            CharacterCreationUI.NewCharacterEvent += Clear;   
        }


        public void Clear()
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);
            for(int i = buttons.Length - 1; i > -1; i--)
            {
                Destroy(buttons[i].gameObject);
            }
        }


        public void PopulateColors(ColorSet colorSet, Mode setMode)
        {
            Clear();
            mode = setMode;
            AvatarColorButton[] colorButtons = new AvatarColorButton[colorSet.Colors.Length];
            for(int i = 0; i < colorButtons.Length; i++)
            {
                GameObject newButton = Instantiate(colorButtonPrefab, transform);
                newButton.GetComponent<AvatarColorButton>().SetColor(colorSet.GetColor(i), i, this);
            }
        }


        public void SetColor(int index) => avatarCreateUI.SetColor(index, mode);


        public void PopulateParts(GameObject avatar, Mode subMode)
        {
            Clear();
            if(!avatar.TryGetComponent<UICharacter>(out var character)) return;
            UICharacter.Part partType;
            mode = subMode;
            switch(subMode)
            {
                case Mode.hairs: partType = UICharacter.Part.hair; break;
                case Mode.brows: partType = UICharacter.Part.brows; break;
                case Mode.beards: partType = UICharacter.Part.beards; break;
                default: return;
            }
            character.MakePartsButtons(partType, partsButtonPrefab, transform);
        }
        public void PopulateHairs(GameObject avatar) => PopulateParts(avatar, Mode.hairs);
        public void PopulateBrows(GameObject avatar) => PopulateParts(avatar, Mode.brows);
        public void PopulateBeards(GameObject avatar) => PopulateParts(avatar, Mode.beards);


        public void RepopulatePartsForSex(GameObject avatar)
        {
            if((mode == Mode.hairs) || (mode == Mode.brows) || (mode == Mode.beards)) PopulateParts(avatar, mode);
            if((mode == Mode.shirts) || (mode == Mode.pants) || (mode == Mode.shoes)) PopulateCloths(avatar, mode);
        }


        public void PopulateCloths(GameObject avatar, Mode subMode)
        {
            Clear();
            if(!avatar.TryGetComponent<UICharacter>(out var character)) return;
            UICharacter.Clothing partType;
            mode = subMode;
            switch(subMode)
            {
                case Mode.shirts: partType = UICharacter.Clothing.shirts; break;
                case Mode.pants: partType = UICharacter.Clothing.pants; break;
                case Mode.shoes: partType = UICharacter.Clothing.shoes; break;
                default: return;
            }
            character.MakeClothingButtons(partType, clothsButtonPrefab, transform);
        }
        public void PopulateShirts(GameObject avatar) => PopulateCloths(avatar, Mode.shirts);
        public void PopulatePants(GameObject avatar) => PopulateCloths(avatar, Mode.pants);
        public void PopulateShoes(GameObject avatar) => PopulateCloths(avatar, Mode.shoes);



        

    }

}
