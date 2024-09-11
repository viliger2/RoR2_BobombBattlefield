using R2API.Utils;
using RegigigasMod.Modules;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
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

        // you can't make skindue to how skin system works
        // we need to have the same number of render infos and meshes as base model
        // regi has two, king bobomb has like 8 (each sprite is a render info)
        // the dream is dead.
        public static void AddKingBobombSkin()
        {
            //var body = BodyCatalog.FindBodyPrefab("RegigigasPlayerBody");
            //if(!body)
            //{
            //    return;
            //}

            //if(!body.TryGetComponent<ModelLocator>(out var modelLocator))
            //{
            //    return;
            //}

            //var model = modelLocator.modelTransform.gameObject;
            //if (!model)
            //{
            //    return;
            //}

            //if(!model.TryGetComponent<ModelSkinController>(out var skinController))
            //{
            //    return;
            //}

            //var skinDef = CreateSkinDef(model, skinController.skins[0]);
            //Array.Resize(ref skinController.skins, skinController.skins.Length + 1);

            //var skinsField = Reflection.GetFieldValue<SkinDef[][]>(typeof(BodyCatalog), "skins");
            //skinsField[(int)BodyCatalog.FindBodyIndex(body)] = skinController.skins;
            //Reflection.SetFieldValue(typeof(BodyCatalog), "skins", skinsField);
        }

        private static SkinDef CreateSkinDef(GameObject modelTransform, SkinDef baseSkin)
        {
            var skinDef = ScriptableObject.CreateInstance<SkinDef>();
            //// fuck me
            //var skinDef = ScriptableObject.Instantiate(Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Beetle/skinBeetleDefault.asset").WaitForCompletion());
            //(skinDef as ScriptableObject).name = "sdRegigigasKingBobomb";
            //skinDef.baseSkins = Array.Empty<SkinDef>();
            //skinDef.icon = null;
            //skinDef.nameToken = "";
            //skinDef.unlockableDef = null;
            //skinDef.rootObject = null;
            //skinDef.rendererInfos = Array.Empty<CharacterModel.RendererInfo>();
            //skinDef.gameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
            //skinDef.meshReplacements = Array.Empty<SkinDef.MeshReplacement>();
            //skinDef.projectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>();
            //skinDef.minionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>();
            //skinDef.runtimeSkin = null;
            //skinDef.skinIndex = SkinIndex.None;
            //// and fuck the one reading this for good measure

            //var characterModel = SM64BBFContent.KingBobomb.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>();

            ////skinDef.icon = LoadoutAPI.CreateSkinIcon(Color.black, Color.white, new Color(0.5F, 0.3F, 0), Color.white); TODO
            //skinDef.nameToken = "SM64_BBF_KING_BOBOMB_BODY_NAME";
            ////skinDef.unlockableDef = ; TODO
            //skinDef.rootObject = modelTransform;
            //skinDef.baseSkins = new SkinDef[] { baseSkin };
            //skinDef.rendererInfos = characterModel.baseRendererInfos; // TODO
            //skinDef.gameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
            //skinDef.meshReplacements = Array.Empty<SkinDef.MeshReplacement>();
            //skinDef.projectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>();
            //skinDef.minionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>();
            //skinDef.runtimeSkin = null;
            //skinDef.skinIndex = SkinIndex.None;

            //skinDef.Bake();

            return skinDef;
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
