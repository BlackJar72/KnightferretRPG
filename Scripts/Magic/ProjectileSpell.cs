using UnityEngine;

namespace kfutils.rpg {


    [CreateAssetMenu(menuName = "KF-RPG/Magic/Projectile Spell Prototype", fileName = "ProjectileCast", order = 20)]
    public class ProjectileSpell : ASpellCast {

        [SerializeField] GameObject projectilePrefab;

        public override void Cast(IAttacker caster) {
            AimParams aimParams;
            caster.GetAimParams(out aimParams);
            GameObject projectileObj = Instantiate(projectilePrefab);
            projectileObj.transform.position = aimParams.from;
            projectileObj.transform.parent = projectileObj.transform.root; 
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if(projectile != null) {
                if(projectile is SpellProjectile spellProj) {
                    spellProj.SetRange(spell.Range, aimParams.from);
                }
                projectile.Launch(caster, aimParams.toward);
            }

        }



    }

}
