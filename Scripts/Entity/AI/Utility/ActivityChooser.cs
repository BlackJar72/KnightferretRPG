using UnityEngine;
using System.Collections.Generic;


namespace kfutils.rpg
{

    [System.Serializable]
    public class ActivityChooser
    {
        [SerializeField] List<ActivityHolder> choices = new List<ActivityHolder>();
        [SerializeField] IActivityObject currentActivity;
        

        // TODO: Add queue from queued activities (possibly using my new RingDeque).


        public IActivityObject Choose()
        {
            if (choices.Count < 1) return null;
            if (choices.Count == 1)
                return currentActivity = choices[0].ActivityObject;
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
            return currentActivity = choices[selection].ActivityObject;
        }


        public void PopulateActivityList()
        {
            //TODO!
        }
        

        /* TODO:
            I need a good way to populate the list, sometimes from multiple sources. 
            Items are simple, poplate from the characters inventories.  Objects in 
            the world ("props") would need to be found from the world; having them 
            register themselves with the chunk manager and search from there would 
            almost certainly be better than search the whole world no matter how 
            big.  Weapons are a special case of items, and can be done similarly. 
            Other / special cases (e.g. spell, action involving only the character) 
            could would be another case to consider.

           IDEA:
            I could optomize during population by replacing the list and the call to 
            sort with a kind of insertion sort during the population phase, useing 
            and array of six element and keeping only those that could be selected. 
            The list would be short to iterate during insertion, and could write 
            into the elements (a kind of object pool) to avoid allocation and 
            garbage collection.
        */



    }


}