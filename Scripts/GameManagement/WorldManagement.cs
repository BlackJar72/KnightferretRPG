using kfutils.rpg;
using UnityEngine;

public static class WorldManagement {

    private static Worldspace worldspace;


    public static Worldspace CurWorldspace => worldspace;
    public static float SeaLevel => worldspace == null ? float.MinValue : worldspace.SeaLevel;


    public static void SetWorldspace(Worldspace world) {
        worldspace = world;
    }


}
