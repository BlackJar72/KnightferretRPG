using UnityEngine;
using System.Collections.Generic;


namespace kfutils.rpg
{

    [System.Serializable]
    public class ActivityChooser
    {
        [SerializeField] List<ActivityHolder> choices = new List<ActivityHolder>();
        [SerializeField] ActivityHolder currentActivity;
        

        // TODO: Add queue from queued activities (possibly using my new RingDeque).


        public ActivityHolder Choose()
        {
            if (choices.Count < 1) return null;
            if (choices.Count == 1)
                return currentActivity = choices[0];
            choices.Sort();
            int numToConsider = choices.Count;
            if (choices.Count > 3)
            {
                numToConsider = Mathf.Min(Mathf.Max((choices.Count / 5), 2), 6);
            }
            float selector = 0;
            for (int i = 0; i < numToConsider; i++)
            {
                selector += choices[i].Utility;
            }
            selector = Random.Range(0, selector);
            int selection = 0;
            while (selector > choices[selection].Utility)
            {
                selector -= choices[selection].Utility;
                selection++;
#if UNITY_EDITOR
                //Testing Failsage
                if (selection >= numToConsider)
                {
                    selection = 0;
                    selector = 0.0f;
                    Debug.LogError("ActivityChooser.Choose(): Selector overran range; something is wrong!");
                    break;
                }
#endif
            }
            return currentActivity = choices[selection];
        }


        public void PopulateActivityList(ITalkerAI ai, params List<IActivityObject>[] activities)
        {
            choices.Clear();
            for (int i = 0; i < activities.Length; i++)
            {
                foreach (IActivityObject prop in activities[i])
                {
                    choices.Add(prop.GetActivityOption(ai));
                }
            }
            foreach (SelfActivity selfActivity in ai.SelfActivities)
                {
                    choices.Add(selfActivity.GetActivityOption(ai));
                }
        }
        

        /* IDEA:
            I could optomize during population by replacing the list and the call to 
            sort with a kind of insertion sort during the population phase, useing 
            and array of six element and keeping only those that could be selected. 
            The list would be short to iterate during insertion, and could write 
            into the elements (a kind of object pool) to avoid allocation and 
            garbage collection.
        */



    }


}