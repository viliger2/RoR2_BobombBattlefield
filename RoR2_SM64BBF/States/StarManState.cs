using EntityStates;
using RoR2;
using RoR2.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SM64BBF.States
{
    public class StarManState : GenericCharacterMain
    {
        public int duration = 20; // its around 10.6 in SMB1, might need to increase it for fun factor

        public static AnimationCurve animCurve = null;

        public static GameObject starmanKillsEffect;

        private TemporaryOverlayInstance temporaryOverlay;

        private BlastAttack blastAttack;

        private bool stopped;

        private AsyncOperationHandle<Material> operationHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.Immune, duration);
            }
            Util.PlaySound("SM64_BBF_Play_StarmanComes", gameObject);
            if (animCurve == null)
            {
                animCurve = AnimationCurve.Linear(0, 0, 1, 1);
                animCurve.postWrapMode = WrapMode.PingPong;
                animCurve.preWrapMode = WrapMode.PingPong;
            }
            blastAttack = CreateBlastAttack();
            operationHandle = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common.matFlashWhite_mat);
            if (operationHandle.IsValid())
            {
                operationHandle.Completed += (operationResult) =>
                {
                    if (operationResult.Status == AsyncOperationStatus.Succeeded)
                    {
                        var model = GetModelTransform();
                        if (model)
                        {
                            temporaryOverlay = TemporaryOverlayManager.AddOverlay(base.gameObject);
                            temporaryOverlay.duration = 0.2f;
                            temporaryOverlay.alphaCurve = animCurve;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.originalMaterial = operationResult.Result;
                            temporaryOverlay.AddToCharacterModel(model.GetComponent<CharacterModel>());
                        }
                    }
                    else
                    {
                        Addressables.Release(operationHandle);
                    }
                };
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (blastAttack != null)
                {
                    blastAttack.position = transform.position;
                    blastAttack.Fire();
                }
            }
            if (fixedAge > duration * 0.75f && !stopped)
            {
                if (temporaryOverlay != null)
                {
                    temporaryOverlay.duration = 0.6f;
                    Util.PlaySound("SM64_BBF_Stop_StarmanComes", gameObject);
                    stopped = true;
                }
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private BlastAttack CreateBlastAttack()
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = characterBody.bestFitActualRadius * 2f;
            blastAttack.procCoefficient = 0f;
            blastAttack.attacker = base.gameObject;
            blastAttack.inflictor = base.gameObject;
            blastAttack.baseForce = 0f;
            blastAttack.bonusForce = Vector3.zero;
            blastAttack.baseDamage = 999999f;
            blastAttack.canRejectForce = false;
            blastAttack.crit = false;
            blastAttack.procChainMask = default;
            blastAttack.damageColorIndex = DamageColorIndex.WeakPoint;
            blastAttack.damageType = DamageType.BypassArmor & DamageType.BypassBlock;
            blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
            blastAttack.teamIndex = TeamIndex.Neutral;
            blastAttack.impactEffect = EffectCatalog.FindEffectIndexFromPrefab(starmanKillsEffect);

            return blastAttack;
        }

        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound("SM64_BBF_Stop_StarmanComes", gameObject);
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.Immune))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.Immune);
                }
            }
            if (temporaryOverlay != null)
            {
                //temporaryOverlay.RemoveFromCharacterModel();
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.Destroy();
                temporaryOverlay = null;
            }
            if (operationHandle.IsValid())
            {
                Addressables.Release(operationHandle);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death; // only falling into the pit can stop the starman, also stops some movement abilities
        }
    }
}
