using System.Collections.Generic;
using UnityEngine;


namespace kfutils.rpg
{

    public class ActivityHolder : IComparer<ActivityHolder>, System.IComparable<ActivityHolder> 
    {
        // Probably will not be extended, but just incase.
        protected IActivityObject activityObject;
        protected float desirability;

        public IActivityObject ActivityObject => activityObject;
        public float Utility => desirability;


        public ActivityHolder(IActivityObject activity, float utility)
        {
            activityObject = activity;
            desirability = utility;
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

        
    }


}
