using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;
using EntityStates;
using RoR2.Audio;

namespace SM64BBF.States
{
    public class BobombExplodeState : GenericCharacterMain
    {
        public int duration = 5;

        public static AnimationCurve animCurve = null;
        public static AnimationCurve sizeCurve = null;

        public bool fromAcquireTarget = false;

        private static GameObject smokeEmitter = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/snowyforest/SFFirepit.prefab").WaitForCompletion().transform.Find("SFFire/HeatGas").gameObject;
        private static GameObject explosionEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXCommandoGrenade");
        private static Material overlayMaterial = LegacyResourcesAPI.Load<Material>("Materials/matFlashWhite");
        public GameObject smoke;

        public TemporaryOverlay temporaryOverlay;
        public ObjectScaleCurve scale;

        public override void OnEnter()
        {
            base.OnEnter();
            if (animCurve == null)
            {
                animCurve = AnimationCurve.Linear(0, 0, 1, 1);
                animCurve.postWrapMode = WrapMode.PingPong;
                animCurve.preWrapMode = WrapMode.PingPong;
            }
            if (sizeCurve == null)
            {
                sizeCurve = AnimationCurve.Linear(0, 1f, 1, 1.25f);
                sizeCurve.postWrapMode = WrapMode.ClampForever;
                sizeCurve.preWrapMode = WrapMode.ClampForever;
            }
            var model = GetModelTransform()?.GetComponent<CharacterModel>();
            if (model)
            {
                temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.2f;
                temporaryOverlay.alphaCurve = animCurve;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.originalMaterial = overlayMaterial;
                temporaryOverlay.AddToCharacerModel(model);
            }
            var cl = GetModelChildLocator();
            var head = cl?.FindChild("Head");

            smoke = GameObject.Instantiate(smokeEmitter, head ?? characterBody.coreTransform);
            var particleSystem = smoke.GetComponent<ParticleSystem>();
            var particleMain = particleSystem.main;
            particleMain.simulationSpace = ParticleSystemSimulationSpace.World;
            smoke.transform.localPosition = Vector3.zero;
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Play_Bobomb_Fuse", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterBody.isSprinting = true;
            characterBody.inputBank.moveVector = characterDirection.forward;
            if ((duration - fixedAge) <= 1 && !scale)
            {
                scale = GetModelTransform()?.gameObject.AddComponent<ObjectScaleCurve>();
                if (scale)
                {
                    scale.overallCurve = sizeCurve;
                    scale.useOverallCurveOnly = true;
                    scale.timeMax = 1;
                }
            }
            if (fixedAge >= duration && isAuthority)
            {
                new BlastAttack
                {
                    radius = 5f,
                    attacker = this.gameObject,
                    teamIndex = teamComponent.teamIndex,
                    crit = RollCrit(),
                    baseDamage = damageStat * 5f,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = 1f,
                    position = characterBody.corePosition
                }.Fire();
                
                EffectManager.SpawnEffect(explosionEffect, new EffectData
                {
                    origin = characterBody.corePosition,
                    scale = 5f
                }, true);
                outer.SetNextState(new BobombSuicideDeathState());
                //outer.SetNextStateToMain();
                //healthComponent.Suicide();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (temporaryOverlay)
            {
                temporaryOverlay.RemoveFromCharacterModel();
                GameObject.Destroy(temporaryOverlay);
            }
            if (scale)
            {
                scale.Reset();
                GameObject.Destroy(scale);
            }
            GameObject.Destroy(smoke);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if(fromAcquireTarget)
            {
                return InterruptPriority.Death;
            }
            return InterruptPriority.Skill;
        }
    }


}
