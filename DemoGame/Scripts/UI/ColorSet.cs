using UnityEngine;
using kfutils.rpg;


namespace rpg.verslika {


    [CreateAssetMenu(menuName = "KF-RPG/Verslika/Character Color Set", fileName = "ColorSet", order = 10)]
    public class ColorSet : ScriptableObject
    {
        [System.Serializable]
        public struct ColorEntry
        {
            [SerializeField] Color color;
            [SerializeField] int row, column;
            public readonly Color Tone => color;
            public readonly int Row => row;
            public readonly int Column => column; 
        }


        [SerializeField] int defaultEntry;
        [SerializeField] ColorEntry[] colors = new ColorEntry[16];


        public int DefaultEntry => defaultEntry;
        public ColorEntry[] Colors => colors;
        public ColorEntry DefaultColor => colors[defaultEntry];


        public ColorEntry GetColor(int index) =>  colors[index];


    }


}

