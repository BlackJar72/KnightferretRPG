using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



namespace kfutils.rpg
{


#region Small Data Packets for Storage
    /// <summary>
    /// A flexible way to conveniently store data of various types; most often 
    /// used for bools, but also potential integer and enum data (enums that 
    /// are meant to be stored this way should have their own implicit conversion). 
    /// Basically an alternative to a C/C++ union, since those are not available in 
    /// C#. This is expect to store for a variety of things, from the state of 
    /// IInteractble game objects in the world to states of the world (e.g., effects) 
    /// of completing a quest).  Enums intended to be saved here should have a 
    /// default state with a value of 0 which can represent the state when this 
    /// has not been set, as retrieving unset data will return 0 (aka, false).
    /// 
    /// There is still the question of data type to store, as the full range of ints 
    /// may not be needed. 
    /// </summary>
    [Serializable]
    public struct GData
    {
        [ES3Serializable] readonly int data; 

        public static implicit operator bool(GData dat) => (dat.data & 0x1) == 1;
        public static implicit operator byte(GData dat) => (byte)dat.data;
        public static implicit operator short(GData dat) => (short)dat.data;
        public static implicit operator int(GData dat) => dat.data;
        public static implicit operator long(GData dat) => dat.data;
        public static explicit operator float(GData dat) => dat.Float;

        public static implicit operator GData(bool dat) => new(dat);
        public static implicit operator GData(byte dat) => new(dat);
        public static implicit operator GData(short dat) => new(dat);
        public static implicit operator GData(int dat) => new(dat);
        public static implicit operator GData(long dat) => new(dat);
        public static explicit operator GData(float dat) => new(dat);

        public GData(byte dat) => data = dat; 
        public GData(short dat) => data = dat;
        public GData(int dat) => data = dat;
        public GData(long dat) => data = (int)dat;   
        // Using fixed point repressentation; simplicity at the cost of precision;
        // May try converting the bits later.
        public GData(float dat) => data = (int)(dat * ObjectManagement.MUL);  
        public GData(bool dat)
        {
            if(dat) data = 0x1;
            else data = 0x0;
        }

        public override readonly string ToString() => data.ToString();
        public readonly float Float => data * ObjectManagement.DIV;
    }


    /// <summary>
    /// A version of GData for things that may expire.  A good example use 
    /// case would be applying this to doors which might stay open during a 
    /// world space transition but be closed again of the player character 
    /// is away for a whole day.
    /// 
    /// This is intended for checking when a previusly visited world space is 
    /// reloaded after leaving and returning.  It is not intended to be used 
    /// as a timer for game objects which are currently loaded; for creating 
    /// a timer either put the timer on the game object (or a similar 
    /// traditional approach) or create a more robust data structure that can 
    /// "live" in an updating registry (list) as is done with health/stamina/mana. 
    /// </summary>
    [Serializable]
    public struct GDataExpiring
    {
        public static readonly GDataExpiring Null = new(0, double.NaN);

        [ES3Serializable] readonly int data; 
        [ES3Serializable] readonly double expiration;

        public static implicit operator bool(GDataExpiring dat) => (dat.data & 0x1) == 1;
        public static implicit operator byte(GDataExpiring dat) => (byte)dat.data;
        public static implicit operator short(GDataExpiring dat) => (short)dat.data;
        public static implicit operator int(GDataExpiring dat) => dat.data;
        public static implicit operator long(GDataExpiring dat) => dat.data;
        public static explicit operator float(GDataExpiring dat) => dat.Float;

        public bool Expired => WorldTime.time > expiration;

        public GDataExpiring(byte dat, double timeOut) { data = dat; expiration = timeOut; }
        public GDataExpiring(short dat, double timeOut) { data = dat; expiration = timeOut; }
        public GDataExpiring(int dat, double timeOut) { data = dat; expiration = timeOut; }  
        // Using fixed point repressentation; simplicity at the cost of precision;
        // May try converting the bits later.
        public GDataExpiring(float dat, double timeOut) { data = (int)(dat * ObjectManagement.MUL); expiration = timeOut; }
        public GDataExpiring(bool dat, double timeOut)
        {
            if(dat) data = 0x1;
            else data = 0x0;
            expiration = timeOut;
        }

        public override readonly string ToString() => "(data: " + data.ToString() + ", experiation: " + expiration + ")";
        public readonly float Float => data * ObjectManagement.DIV;
    }
#endregion Small Data Packets for Storage


    public static class ObjectManagement
    {
        public const float MUL = 656636.0f;
        public const float DIV = 1.0f / MUL;

        private static Dictionary<string, GData> worldObjectData;
        private static Dictionary<string, GDataExpiring> worldTimedData;

        private static Dictionary<string, GData> tmpWorldObjectData;
        private static Dictionary<string, GDataExpiring> tmpWorldTimedData;

        public static Dictionary<string, GData> WorldObjectData => worldObjectData;
        public static Dictionary<string, GDataExpiring> WorldTimedData => worldTimedData;


        public static void SetData(string id, GData state)
        {
            if (worldObjectData.ContainsKey(id)) worldObjectData[id] = state;
            else worldObjectData.Add(id, state);
        }


        public static GData GetData(string id)
        {
            if(worldObjectData.ContainsKey(id)) return worldObjectData[id];
            return 0;
        } 


        public static void SetTimedData(string id, GDataExpiring state)
        {
            if (worldTimedData.ContainsKey(id)) worldTimedData[id] = state;
            else worldTimedData.Add(id, state);
        }


        public static GDataExpiring GetTimedData(string id)
        {

            if(worldTimedData.ContainsKey(id)) {
                GDataExpiring result = worldTimedData[id];
                if(result.Expired)
                {
                    worldTimedData.Remove(id);
                    return GDataExpiring.Null;
                }
                return worldTimedData[id];
            }
            return GDataExpiring.Null;
        } 


// #if UNITY_EDITOR
//         public static void TimedDataStringDEBUG()
//         {
//             StringBuilder sb = new("WorldTimedData (" + worldTimedData.GetHashCode() + ") [" + Environment.NewLine);
//             foreach (KeyValuePair<string, GDataExpiring> entry in worldTimedData) sb.Append(" \t " + entry.Key + ":  " + entry.Value.ToString() + Environment.NewLine);
//             sb.Append("]" + Environment.NewLine);
//             Debug.Log(sb.ToString());
//         }
//         public static void TimedDataStringDEBUG(Dictionary<string, GDataExpiring> FUCK  )
//         {
//             StringBuilder sb = new("WorldTimedData (" + FUCK.GetHashCode() + ") [" + Environment.NewLine);
//             foreach (KeyValuePair<string, GDataExpiring> entry in FUCK) sb.Append(" \t " + entry.Key + ":  " + entry.Value.ToString() + Environment.NewLine);
//             sb.Append("]" + Environment.NewLine);
//             Debug.Log(sb.ToString());
//         }
// #endif


        public static void NewGame()
        {
            worldObjectData = new();
            worldTimedData = new();
        }


        public static void LoadData(Dictionary<string, GData> data, Dictionary<string, GDataExpiring> timedData)
        {
            tmpWorldObjectData = data;
            tmpWorldTimedData = timedData;
        }


        public static void FinishLoadData()
        {
            worldObjectData = tmpWorldObjectData;
            worldTimedData = tmpWorldTimedData;
        }




    }



}