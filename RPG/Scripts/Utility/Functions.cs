using UnityEngine;

namespace kfutils.rpg {

    public static class Functions {
        
        public static Damages CalcFallDamage(float vSpeed, int naturalArmor) {
            vSpeed = -vSpeed;
            vSpeed = Mathf.Max((vSpeed - 8.0f) / GameConstants.GRAVITY3, 0);
            vSpeed *= vSpeed;
            int dmgBase = Mathf.FloorToInt(vSpeed * 8.0f);
            return DamageUtils.CalcDamage(dmgBase, naturalArmor);
        }

    }


}