using System;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Henry.HenryStates
{
    //TODO:
    //Fix the combo finisher being weird.

    class HenryMeleeAttack : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {

        public int step = 0;
        public static float recoilAmplitude = 0.5f;
        public static float baseDurationBeforeInterruptable = 0.5f;
        public float bloom = 1f;
        /*public static float comboFinisherBaseDuration = 0.5f;
        public static float comboFinisherhitPauseDuration = 0.15f;
        public static float comboFinisherBloom = 0.5f;
        public static float comboFinisherBaseDurationBeforeInterruptable = 0.5f;
        private string animationStateName;*/
        private float durationBeforeInterruptable;

        private bool isComboFinisher
        {
            get
            {
                return this.step == 2;
            }
        }

        public override bool allowExitFire
        {
            get
            {
                return base.characterBody && !base.characterBody.isSprinting;
            }
        }



        public override void OnEnter()
        {
            this.hitBoxGroupName = "SwordHitbox";
            this.baseDuration = 1f;
            this.duration = this.baseDuration / base.attackSpeedStat;
            this.hitPauseDuration = 0.1f;
            this.damageCoefficient = 2f;

            hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniimpactvfxmedium");
            swingEffectMuzzleString = "SwingCenter";

            if (isComboFinisher)
            {
                //LogCore.LogF("finisher");
                this.hitBoxGroupName = "TempHitboxLarge";
                //this.baseDuration = 1f;
                //this.duration = this.baseDuration / base.attackSpeedStat;
                this.hitPauseDuration = 0.2f;
                this.damageCoefficient = 4f;
            }
            //else { LogCore.LogW("not finisher"); }

            base.OnEnter();
            base.characterDirection.forward = base.GetAimRay().direction;
            this.durationBeforeInterruptable = baseDurationBeforeInterruptable / this.attackSpeedStat;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType = DamageType.Generic;
        }

        public override void PlayAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "Slash", "Slash.playbackRate", duration, 0.05f);
        }

        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            base.characterBody.AddSpreadBloom(this.bloom);
        }

        public override void BeginMeleeAttackEffect()
        {
            //this.swingEffectMuzzleString = this.animationStateName;
            //base.AddRecoil(-0.1f * BladeOfCessation.recoilAmplitude, 0.1f * BladeOfCessation.recoilAmplitude, -1f * BladeOfCessation.recoilAmplitude, 1f * BladeOfCessation.recoilAmplitude);
            base.BeginMeleeAttackEffect();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)this.step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.step = (int)reader.ReadByte();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.durationBeforeInterruptable)
            {
                return InterruptPriority.Skill;
            }
            return InterruptPriority.PrioritySkill;
        }

        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            this.step = i;
        }
    }
}

