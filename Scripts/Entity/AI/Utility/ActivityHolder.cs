using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    //[System.Serializable]
    public sealed class ActivityHolder : IComparer<ActivityHolder>, System.IComparable<ActivityHolder>
    {
        // Probably will not be extended, but just incase.
        /*[SerializeField]*/ private IActivityObject activityObject;
        /*[SerializeField]*/ private float desirability;
        /*[SerializeField]*/ private ItemStack theItemStack;

        public IActivityObject ActivityObject => activityObject;
        public float Utility => desirability;
        public ItemStack itemStack => theItemStack;
        public bool IsItem => theItemStack != null;


        public ActivityHolder(IActivityObject activity, float utility, ItemStack item = null)
        {
            activityObject = activity;
            desirability = utility;
            theItemStack = item;
        }


        // Comparisons -- desirability is the basis, since the more desirable choices willl be preferred
        // Compare() and CompareTo() are set up so that sort functions will place higher desirability on top
        public static bool operator >(ActivityHolder a, ActivityHolder b) => a.desirability > b.desirability;
        public static bool operator <(ActivityHolder a, ActivityHolder b) => a.desirability < b.desirability;
        public static bool operator >=(ActivityHolder a, ActivityHolder b) => a.desirability >= b.desirability;
        public static bool operator <=(ActivityHolder a, ActivityHolder b) => a.desirability <= b.desirability;


        public int Compare(ActivityHolder a, ActivityHolder b)
        {
            return -a.desirability.CompareTo(b.desirability); // Should I do this, or use arithmetic?
        }


        public int CompareTo(ActivityHolder other)
        {
            return -desirability.CompareTo(other.desirability); // Should I do this, or use arithmetic?
        }


        public ActivityHolder Duplicate() => new ActivityHolder(activityObject, desirability);


        public void CopyInto(ActivityHolder other)
        {
            activityObject = other.activityObject;
            desirability = other.desirability;
        }


        /// <summary>
        /// Writes the data into an existing instance, which may be used in a possible optimization.
        /// </summary>
        /// <param name="activityHolder"></param>
        /// <param name="activity"></param>
        /// <param name="utility"></param>
        public static void Overwrite(ref ActivityHolder activityHolder, IActivityObject activity, float utility)
        {
            activityHolder.activityObject = activity;
            activityHolder.desirability = utility;
        }


    }


}
