using UnityEngine;


namespace kfutils.rpg {


    /// <summary>
    /// Constants used throught the game, for example, the game layers.
    /// </summary>
    public static class GameConstants
    {
        public const float GRAVITY = 9.8f;
        public const float GRAVITY3 = 3.27f; // One third gravity

        public const float TIME_SCALE = 60f; // How many times faster time runs in game; 60 give a 24 minute day

        public const float TERMINAL_V = 54.0f;


        #region Layers
        public const int defaultLayer = 0x1;
        public const int postLayer = 0x1 << 3;
        public const int waterLayer = 0x1 << 4;
        public const int interactableLayer = 0x1 << 6;
        public const int pcBodyleLayer = 0x1 << 7;
        public const int pcArmsLayer = 0x1 << 8;
        public const int npcLayer = 0x1 << 9;
        public const int damageableLayer = 0x1 << 11;
        public const int attackLayer = 0x1 << 12;
        public const int skyLayer = 0x1 << 20;

        public const int interactable = interactableLayer | npcLayer | defaultLayer; // Includes default so you can't reach through objects
        public const int LevelMask = defaultLayer | interactableLayer;
#endregion



    }


}
