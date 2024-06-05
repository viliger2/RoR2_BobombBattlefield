using EntityStates;
using RoR2.Audio;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Diagnostics;

namespace SM64BBF.States
{
    public class StarManState : GenericCharacterMain
    {
        public int duration = 20; // its around 10.6 in SMB1, might need to increase it for fun factor

        public static AnimationCurve animCurve = null;

        public TemporaryOverlay temporaryOverlay;

        private readonly List<HealthComponent> ignoredHealthComponentList = new List<HealthComponent>();

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.Immune, duration);
                EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Play_StarmanComes", gameObject);
            }
            if (animCurve == null)
            {
                animCurve = AnimationCurve.Linear(0, 0, 1, 1);
                animCurve.postWrapMode = WrapMode.PingPong;
                animCurve.preWrapMode = WrapMode.PingPong;
            }

            var model = GetModelTransform()?.GetComponent<CharacterModel>();
            if (model)
            {
                temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.2f;
                temporaryOverlay.alphaCurve = animCurve;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matFlashWhite");
                temporaryOverlay.AddToCharacerModel(model);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!base.isAuthority)
            {
                Collider[] array = Physics.OverlapBox(transform.position, transform.lossyScale * 0.5f, transform.rotation, LayerIndex.entityPrecise.mask);
                foreach (Collider collider in array)
                {
                    var hurtBox = collider.GetComponent<HurtBox>();
                    if (hurtBox && HurtBoxPassesFilter(hurtBox))
                    {
                        ignoredHealthComponentList.Add(hurtBox.healthComponent);
                        EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_StarmanKills", gameObject);
                        if (NetworkServer.active)
                        {
                            hurtBox.healthComponent.Suicide();
                        }
                        else
                        {
                            CmdKill(hurtBox.healthComponent?.body?.networkIdentity?.netId ?? default(NetworkInstanceId));
                        }
                    }
                }
            }
            if (fixedAge > duration * 0.75f)
            {
                temporaryOverlay.duration = 0.4f;
                EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Stop_StarmanComes", gameObject);
            }

            if (fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        [Command]
        public void CmdKill(NetworkInstanceId netId) 
        {
            if(netId == default(NetworkInstanceId))
            {
                return;
            }
            var gameObject = Util.FindNetworkObject(netId);
            if(gameObject)
            {
                if(gameObject.TryGetComponent<CharacterBody>(out var body))
                {
                    body.healthComponent.Suicide();
                }
            }
        }

        private bool HurtBoxPassesFilter(HurtBox hurtBox)
        {
            if (!hurtBox.healthComponent)
            {
                return true;
            }
            if (hurtBox.healthComponent.gameObject == gameObject)
            {
                return false;
            }
            if (ignoredHealthComponentList.Contains(hurtBox.healthComponent))
            {
                return false;
            }
            return true;
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Stop_StarmanComes", gameObject);
                if (characterBody.HasBuff(RoR2Content.Buffs.Immune))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.Immune);
                }
            }
            temporaryOverlay.RemoveFromCharacterModel();
            UnityEngine.Object.Destroy(temporaryOverlay);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill; // only falling into the pit can stop the starman, also stops some movement abilities
        }
    }
}
