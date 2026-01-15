using kfutils.rpg;
using kfutils.rpg.ui;
using UnityEngine;
 

/// <summary>
/// A temporary class to turn on water in testing.
/// This will be dones in a more flexible and sophisticated way later, which will also allow for other 
/// colors.
/// </summary>
public class WaterVisual : MonoBehaviour {

    [SerializeField] float waterLevel;
    [SerializeField] ScreenFX screen;

    void Update() {
        float waterLevel = WorldManagement.SeaLevel;
        if(transform.position.y < waterLevel) screen.SetColor(ScreenFX.ScreenColor.water);
        else screen.SetColor(ScreenFX.ScreenColor.clear);
        // FIXME: This should be handled elsewhere with water level as a constant (or worldspace setting) not a script setting attached to the camera.
        //        Also, accelleration due to gravity should not apply at this point, so that the player doesn't constantly bob while swimming at the surface.
    }


}
