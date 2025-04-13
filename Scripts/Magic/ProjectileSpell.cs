using UnityEngine;

namespace kfutils.rpg {


    [CreateAssetMenu(menuName = "KF-RPG/Magic/Spell Prototype", fileName = "Projectile Cast", order = 20)]
    public class ProjectileSpell : ASpellCast {

        [SerializeField] GameObject projectile;

        public override void Cast(IActor caster) {
            throw new System.NotImplementedException();
        }



    }

}
