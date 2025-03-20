using UnityEngine;

namespace kfutils.rpg {

    public static class Functions {
        
        public static int CalcFallDamage(float vSpeed) {
            vSpeed = -vSpeed;
            vSpeed = Mathf.Max((vSpeed - 7.0f) / GameConstants.GRAVITY, 0);
            vSpeed *= vSpeed;
            return Mathf.FloorToInt(vSpeed * 8.0f);
        }

    }

}
