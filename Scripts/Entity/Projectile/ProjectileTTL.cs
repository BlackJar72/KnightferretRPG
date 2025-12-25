using System.Collections;
using UnityEngine;

namespace kfutils.rpg {

    public class ProjectileTTL : Projectile
    {
        [SerializeField] float timeToLive = 2.0f;


        public override void Launch(ICombatant sender, Vector3 direction)
        {
            base.Launch(sender, direction);
            StartCoroutine(SelfDestruct());
        }


        public IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(timeToLive);
            Destroy(gameObject);
        }


    }

}


