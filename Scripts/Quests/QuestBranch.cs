using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    [System.Serializable]
    public class QuestBranch
    {
        [SerializeField] QuestLinkage type;
        [SerializeField] List<QuestEdge> options;
        

    }




    [System.Serializable]
    public class QuestEdge
    {

    }

}
