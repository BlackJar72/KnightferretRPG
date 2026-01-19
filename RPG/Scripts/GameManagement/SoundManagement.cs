using UnityEngine;
using System.Collections.Generic;



namespace kfutils.rpg {


    /// <summary>
    /// A class for handling in game sounds heard by NPC. 
    /// 
    /// Note that this is not intended to handle sounds heard by the player, ie, its not about 
    /// playing sounds.  Instead, it is for handling the logic of sound in world. 
    /// 
    /// I don't want sound making tied tightly to the maker or the hearer; Caverns of Evil did 
    /// this in both ways and it caused flexibility problems that kept certain mechanics out of 
    /// the game.
    /// 
    /// The system is simple.  We don't have all possible hearers constantly check for sound. 
    /// Instead, we wait until a relevant sound is made, and let the sound look for hearers to 
    /// notify that they have heard it. 
    /// </summary>
    public static class SoundManagement
    {

        public static List<(EntityLiving, float)> FindHearers(WorldSound sound)
        {
            List<(EntityLiving, float)> hearers = new();
            foreach(EntityLiving entity in EntityManagement.activeEntities)
            {
                float howMuch;
                if(entity.CanHear(sound, out howMuch)) hearers.Add((entity, howMuch));
            }
            return hearers;
        }


        public static void SoundMadeAt(WorldSound sound)
        {
            
        }


        public static void SoundMadeAtBy(WorldSound sound, EntityLiving entity)
        {
            List<(EntityLiving, float)> hearers = FindHearers(sound);
            for(int i = 0; i < hearers.Count; i++)
            {
                hearers[i].Item1.HearSound(sound, hearers[i].Item2);
                // Do more here?
            }
            //Do more here?
        }


        public static void SoundMadeAtByPlayer(WorldSound sound, PCTalking entity)
        {
            List<(EntityLiving, float)> hearers = FindHearers(sound);
            for(int i = 0; i < hearers.Count; i++)
            {
                hearers[i].Item1.HearSound(sound, hearers[i].Item2);
                // Do more here?
            }
            //Do more here?
        }

        
    }


}
