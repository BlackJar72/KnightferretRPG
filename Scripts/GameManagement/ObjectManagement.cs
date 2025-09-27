using UnityEngine;
using System.Collections.Generic;



namespace kfutils.rpg
{

    public class ObjectManagement
    {

        private static Dictionary<string, bool> WorldObjectBools;

        public static void SetBool(string id, bool state)
        {
            if (WorldObjectBools.ContainsKey(id)) WorldObjectBools[id] = state;
            else WorldObjectBools.Add(id, state);
        }

        public static bool GetBool(string id) => WorldObjectBools[id];


    }



}