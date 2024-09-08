using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SM64BBF.States
{
    public class BobombExplodeState : GenericCharacterMain
    {
        public int duration = 5;

        public static AnimationCurve animCurve = null;
        public static AnimationCurve sizeCurve = null;

        private static GameObject smokeEmitter = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/snowyforest/SF_Firepit.prefab").WaitForCompletion().transform.Find("SFFire/HeatGas").gameObject;
        private static GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
        private static Material overlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matFlashWhite.mat").WaitForCompletion();
        public GameObject smoke;

        public TemporaryOverlayInstance temporaryOverlay;
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
                temporaryOverlay = TemporaryOverlayManager.AddOverlay(base.gameObject);
                temporaryOverlay.duration = 0.2f;
                temporaryOverlay.alphaCurve = animCurve;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.originalMaterial = overlayMaterial;
                temporaryOverlay.AddToCharacterModel(model);
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
                    radius = 15f,
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
                //outer.SetNextStateToMain();
                outer.SetNextState(new BobombSuicideDeathState());
            }
        }

        public override void OnExit()
        {
            if (temporaryOverlay != null)
            {
                //temporaryOverlay.RemoveFromCharacterModel();
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.Destroy();
                temporaryOverlay = null;
            }
            if (scale)
            {
                scale.Reset();
                GameObject.Destroy(scale);
            }
            GameObject.Destroy(smoke);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }


}
