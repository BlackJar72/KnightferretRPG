using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;


namespace kfutils.rpg {

    public class ItemThrowable : ItemConsumable
    {
        [Tooltip("The item's thrown form; its hould usually a ThrownWeapon\n but other projectiles are allower for special cases.")] 
        [SerializeField] Projectile projectile;
        [SerializeField] ItemActions throwAnimation;
        [SerializeField] float releaseTime = 0.18f;
        [SerializeField] Sound useSound;
        [SerializeField] AudioSource audioSource;

        private AimParams aim;

        protected bool busy = false;
        protected bool queued = false;
        
        protected AnimancerState throwState;


        public override void OnUse(IActor actor)
        {
            if ((!busy) && (holder is ICombatant thrower))
            {
                useSound.Play(audioSource);
                throwState = thrower.PlayAction(useAnimation.mask, useAnimation.anim, OnUseAnimationEnd, 0, useAnimation.anim.Clip.length);
                PCActing pc = thrower as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
                busy = true;
                StartCoroutine(DelayedLaunch());
            }
        }


        public override void PlayUseAnimation(IActor actor)
        {
            if (holder is ICombatant thrower)
            {
                useSound.Play(audioSource);
                throwState = thrower.PlayAction(useAnimation.mask, useAnimation.anim, OnUseAnimationEnd, 0, useAnimation.anim.Clip.length);
                PCActing pc = thrower as PCActing;
                if (pc != null) pc.SetArmsPos(PCActing.ArmsPos.high);
            }
        }


        private IEnumerator DelayedLaunch()
        {
            yield return new WaitForSeconds(releaseTime);
            LaunchWeapon();
        }


        private void LaunchWeapon()
        {
            if(holder is ICombatant thrower) 
            {
                Vector3 direction = aim.toward;
                Projectile thrown = Instantiate(projectile, transform);
                GameObject thrownObject = thrown.gameObject;
                if(thrown is SpellProjectile spell) spell.SetRange(50, transform.position);
                if(thrown is ThrownItem weapon) weapon.SetItem(prototype);
                if(Physics.Raycast(aim.from, aim.toward, out RaycastHit hitInfo, 64, GameConstants.attackableLayer))
                {
                    direction = hitInfo.point - thrown.transform.position;
                }     
                thrownObject.transform.parent = WorldManagement.WorldLogic.GetChunk(transform.position).gameObject.transform;
                thrownObject.transform.LookAt(transform.position - direction);
                thrown.Launch(thrower, direction.normalized);
                DecrimentSlot();
            }
        }


        public void OnUseAnimationEnd()
        {
            busy = false;
            //attacking = false;
            if (queued)
            {
                queued = false;
                OnUse(holder);
            }           
        }


    }

}
