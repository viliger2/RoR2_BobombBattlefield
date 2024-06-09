using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SM64BBF
{
    public static class RegigigasCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.RegigigasMod");
                }
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SetupKingBobombBody(GameObject characterBody, ContentPack contentPack)
        {
            characterBody.GetComponent<CharacterBody>().preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.SpawnState));

            var components = characterBody.GetComponents<EntityStateMachine>();
            foreach (var component in components)
            {
                if (component.customName == "Body")
                {
                    component.mainStateType = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.MainState));
                    component.initialStateType = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.SpawnState));
                }
            }
            characterBody.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.DeathState));

            characterBody.AddComponent<RegigigasMod.Modules.Components.RegigigasController>();
            characterBody.AddComponent<RegigigasMod.Modules.Components.RegigigasFlashController>();
            characterBody.AddComponent<RegigigasMod.Modules.Components.SlowStartController>();

            var footstepsController = characterBody.GetComponentInChildren<FootstepHandler>();
            footstepsController.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion();
            footstepsController.enableFootstepDust = true;
        }

        public static void SetupPrimarySkill(SkillDef primary)
        {
            SetupSkillActivationState(primary, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.GrabAttempt)));
        }

        public static void SetupSecondarySkill(SkillDef secondary)
        {
            SetupSkillActivationState(secondary, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Stomp)));
        }

        public static void SetupUtilitySkill(SkillDef utility)
        {
            SetupSkillActivationState(utility, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.BounceStart)));
        }

        public static void SetupSpecialSkill(SkillDef special)
        {
            SetupSkillActivationState(special, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Revenge)));
        }

        private static void SetupSkillActivationState(SkillDef skill, EntityStates.SerializableEntityStateType activationState)
        {
            if (skill)
            {
                skill.activationState = activationState;
            }
            else
            {
                Log.Warning("KingBobomb: Skill for activation state " + activationState + "doesn't exist.");
            }
        }
    }
}
