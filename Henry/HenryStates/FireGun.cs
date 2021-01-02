using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Henry.HenryStates
{
    public class FireGun : BaseSkillState
    {
        //note, no one is going to set these variables for you. you will have to set them yourself.

        //https://github.com/risk-of-thunder/R2Wiki/wiki/Resources-Paths
        private GameObject tracerEffect = Resources.Load<GameObject>("prefabs/effects/tracers/TracerCaptainDefenseMatrix");
        private GameObject hitEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/CaptainAirstrikeImpact1");
        private GameObject muzzleFlash = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/CaptainAirstrikeMuzzleEffect");
        
        //statics
        public static float baseDuration = 1;
        public static float damage = float.MaxValue;

        //don't set this.
        private float duration;



        private void FireRadicalGun(string targetMuzzle) {
            base.PlayCrossfade("Gesture, Override", "ShootGun", "ShootGun.playbackRate", duration, 0.05f);
            Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleFlash, base.gameObject, targetMuzzle, false);
            if (base.isAuthority)
            {
                var aimRay = GetAimRay();
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    damage = damage * this.damageStat,
                    force = 200 ,
                    tracerEffectPrefab = tracerEffect,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = hitEffect,
                    isCrit = RollCrit(),
                    radius = 0.1f,
                    //controls the fall off of the bullet.
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    smartCollision = true
                }.Fire();
            }
            base.characterBody.AddSpreadBloom(10);

        }
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            var aimRay = GetAimRay();
            base.StartAimMode(aimRay, 3f, false);
            FireRadicalGun("Muzzle");
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            };
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
