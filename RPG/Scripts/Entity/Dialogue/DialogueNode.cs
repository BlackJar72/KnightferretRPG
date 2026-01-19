using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    [CreateAssetMenu(fileName = "DialogueNode", menuName = "KF-RPG/Dialogue/Dialogue Node", order = 10)]
    public class DialogueNode : ScriptableObject
    {

        [SerializeField] string id;
        [SerializeField] string text;
        [SerializeField] List<DialogueOption> responses;


    }

    [System.Serializable]
    public class DialogueOption
    {
        
        public string text;
        public string npcResponseID;


    }


}
