using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RegigigasMod;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
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
                //Debug.Log("EntityStateMachine name " + component.name);
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

            //RegigigasMod.Modules.Enemies.Regigigas.CreateSkills(characterBody, false);
        }

        public static void SetupPrimarySkill(SkillDef primary)
        {
            SetupSkillActivationState(primary, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.GrabAttempt)));
            //primary.activationState = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.GrabAttempt));
        }

        public static void SetupSecondarySkill(SkillDef secondary)
        {
            SetupSkillActivationState(secondary, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Stomp)));
            //secondary.activationState = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Stomp));
        }

        public static void SetupUtilitySkill(SkillDef utility)
        {
            SetupSkillActivationState(utility, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.BounceStart)));
            //utility.activationState = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.BounceStart));
        }

        public static void SetupSpecialSkill(SkillDef special)
        {
            SetupSkillActivationState(special, new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Revenge)));
            //special.activationState = new EntityStates.SerializableEntityStateType(typeof(RegigigasMod.SkillStates.Regigigas.Revenge));
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
