using UnityEngine;
using System.Collections.Generic;


namespace kfutils.rpg
{

    [System.Serializable]
    public class ActivityChooser
    {
        [SerializeField] List<ActivityHolder> choices = new List<ActivityHolder>();
        [SerializeField] IActivityObject currentActivity;




    }


}