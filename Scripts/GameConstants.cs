using UnityEngine;


namespace kfutils.rpg {


    [System.Serializable]
    public enum Layers
    {
        unityDefault = 0,
        post = 3,
        water = 4,
        interactable = 6,
        pcBody = 7,
        pcArms = 8,
        npc = 9,
        projectile = 10,
        damageable = 11,
        attack = 12,
        block = 13,
        smallObject = 14,
        smallItem = 15, 
        door = 16,
        floor = 17,
        settling = 18
    }


    /// <summary>
    /// Constants used throughout the game, for example, the game layers.
    /// </summary>
    public static class GameConstants
    {
        public const float GRAVITY = 9.8f;
        public const float GRAVITY3 = 3.27f; // One third gravity

        public const float TIME_SCALE = 60f; // How many times faster time runs in game; 60 gives a 24 minute day

        public const float TERMINAL_VELOCITY = -54.0f;

        public const float TIME_TO_CHARGE_ACTIONS = 0.5f;

        public const int ENVIRO_DMG_FACTOR = 4;
        public const float ENVIRO_DMG_INV = 1.0f / (float)ENVIRO_DMG_FACTOR;


        #region Layers
        public const int defaultLayer = 0x1;
        public const int postLayer = 0x1 << (int)Layers.post;
        public const int waterLayer = 0x1 << (int)Layers.water;
        public const int interactableLayer = 0x1 << (int)Layers.interactable;
        public const int pcBodyleLayer = 0x1 << (int)Layers.pcBody;
        public const int pcArmsLayer = 0x1 << (int)Layers.pcArms;
        public const int npcLayer = 0x1 << (int)Layers.npc;
        public const int projectileLayer = 0x1 << (int)Layers.projectile;
        public const int damageableLayer = 0x1 << (int)Layers.damageable;
        public const int attackableLayer = damageableLayer | (0x1 << (int)Layers.block);
        public const int attackLayer = 0x1 << (int)Layers.attack;
        public const int smallObjectLayer = 0x1 << (int)Layers.smallObject;
        public const int smallItemLayer = 0x1 << (int)Layers.smallItem;
        public const int doorLayer = 0x1 << (int)Layers.door;
        public const int floorLayer = 0x1 << (int)Layers.floor;
        public const int settlingLayer = 0x1 << (int)Layers.settling;

        public const int interactable = interactableLayer | smallItemLayer | doorLayer | npcLayer | defaultLayer; // Includes default so you can't reach through objects
        public const int LevelMask = defaultLayer | interactableLayer | floorLayer;
        #endregion



    }


}
