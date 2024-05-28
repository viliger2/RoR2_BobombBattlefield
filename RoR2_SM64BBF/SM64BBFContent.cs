using System;
using System.Collections;
using System.Linq;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using RoR2.Networking;
using R2API;
using SM64BBF.PickUpDefs;
using RoR2.Skills;
using UnityEngine.UIElements;
using static RoR2.ItemDisplayRuleSet;
using HG;

namespace SM64BBF
{
    public static class SM64BBFContent
    {
        internal const string ScenesAssetBundleFileName = "sm64bbfstage";
        internal const string AssetsAssetBundleFileName = "sm64bbfassets";

        internal const string SoundsSoundBankFileName = "SM64BBFSounds.bnk";
        internal const string MusicSoundBankFileName = "SM64BBFMusic.bnk";
        internal const string InitSoundBankFileName = "SM64BBFInit.bnk";

        private static AssetBundle _scenesAssetBundle;
        private static AssetBundle _assetsAssetBundle;

        //internal static UnlockableDef[] UnlockableDefs;
        //internal static SceneDef[] SceneDefs;

        internal static SceneDef WHSceneDef;
        internal static Sprite SM64BBFPreviewSprite;
        internal static Material WHBazaarSeer;

        public static List<Material> SwappedMaterials = new List<Material>(); //apparently you need it because reasons?

        public struct MiscPickups
        {
            public static StarmanPickupDef Starman;
            public static CoinPickupDef Coin;
        }
        
        public struct Items
        {
            public static ItemDef MarioOneUp;
        }

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" }
        };

        internal static IEnumerator LoadAssetBundlesAsync(AssetBundle scenesAssetBundle, AssetBundle assetsAssetBundle, IProgress<float> progress, ContentPack contentPack)
        {
            _scenesAssetBundle = scenesAssetBundle;
            _assetsAssetBundle = assetsAssetBundle;
            //Log.Debug($"Asset bundles found. Loading asset bundles...");

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<Material[]>)((assets) =>
            {
                var materials = assets;

                if (materials != null)
                {
                    foreach (Material material in materials)
                    {
                        if (!material.shader.name.StartsWith("Stubbed")) { continue; }

                        var replacementShader = Addressables.LoadAssetAsync<Shader>(ShaderLookup[material.shader.name.ToLower()]).WaitForCompletion();
                        var oldName = material.shader.name.ToLower();
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            SwappedMaterials.Add(material);
                            //Log.Debug("swapped shader " + material.shader.name.ToLower() + "with " + oldName);
                        }
                        else
                        {
                            Log.Warning("Couldn't find replacement shader for " + material.shader.name.ToLower());
                        }
                    }
                }
                //Log.Debug("swapped materials");

            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<UnlockableDef[]>)((assets) =>
            {
                contentPack.unlockableDefs.Add(assets);
                //Log.Debug("loaded unlockableDefs");

            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<Sprite[]>)((assets) =>
            {
                SM64BBFPreviewSprite = assets.First(a => a.name == "texSM64BBFPreview");
                //Log.Debug("loaded preview sprite");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SceneDef[]>)((assets) =>
            {
                //SceneDefs = assets;
                WHSceneDef = assets.First(sd => sd.cachedName == "sm64_bbf_SM64_BBF");
                contentPack.sceneDefs.Add(assets);
                //Log.Debug("loaded SceneDefs");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<MusicTrackDef[]>)((assets) =>
            {
                contentPack.musicTrackDefs.Add(assets);
                //Log.Debug("loaded musicDefs");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<CoinPickupDef[]>)((assets) =>
            {
                MiscPickups.Coin = assets.First(spd => spd.name == "CoinPickUpDef");
                contentPack.miscPickupDefs.Add(assets);
                //Log.Debug("loaded CoinPickupDef");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<StarmanPickupDef[]>)((assets) =>
            {
                //var bossOrb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/BossOrb.prefab").WaitForCompletion();

                MiscPickups.Starman = assets.First(spd => spd.name == "CustomStarmanPickupDef");
                //MiscPickups.Starman.dropletDisplayPrefab = bossOrb;
                contentPack.miscPickupDefs.Add(assets);
                //Log.Debug("loaded StarmanPickupDef");
            }));

            // networkedObjectPrefabs
            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<GameObject[]>)((assets) =>
            {
                var MarioTreeIntractable = assets.First(interactable => interactable.name == "TreeInteractable");
                var RollingRock = assets.First(gameObject => gameObject.name == "RollingRock");
                contentPack.networkedObjectPrefabs.Add(new GameObject[] { MarioTreeIntractable, RollingRock });
                //Log.Debug("loaded networkedObjects");
            }));
            
            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<ItemDef[]>)((assets) =>
            {
                Items.MarioOneUp = assets.First(id => id.name == "OneUp");
                contentPack.itemDefs.Add(assets);
                //Log.Debug("loaded itemDefs");
            }));

            // bodyPrefabs
            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<GameObject[]>)((assets) =>
            {
                var bobombBody = assets.First(bp => bp.name == "BobombBody");

                var bodyComponent = bobombBody.GetComponent<CharacterBody>();
                bodyComponent._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

                var cameraParams = bobombBody.GetComponent<CameraTargetParams>();
                cameraParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandard.asset").WaitForCompletion();

                var footstepsController = bobombBody.GetComponentInChildren<FootstepHandler>();
                footstepsController.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
                footstepsController.enableFootstepDust = true;

                contentPack.bodyPrefabs.Add(new GameObject[] { bobombBody });

                if (SM64BBF.RegigigasCompat.enabled)
                {
                    var kingBobombBody2 = assets.First(bp => bp.name == "KingBobomb2Body");
                    var bodyComponent2 = kingBobombBody2.GetComponent<CharacterBody>();
                    bodyComponent2._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

                    //var kingBobombBody = assets.First(bp => bp.name == "KingBobombBody");
                    //var bodyComponent3 = kingBobombBody.GetComponent<CharacterBody>();
                    //bodyComponent3._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

                    SM64BBF.RegigigasCompat.SetupKingBobombBody(kingBobombBody2, contentPack);

                    contentPack.bodyPrefabs.Add(new GameObject[] { kingBobombBody2 });
                    //contentPack.bodyPrefabs.Add(new GameObject[] { kingBobombBody });
                    //Log.Debug("added and setup RegigigasKingBobombBody");
                }
                //Log.Debug("loaded bodyPrefabs");
            }));

            // masterPrefabs
            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<GameObject[]>)((assets) =>
            {
                var bobombMaster = assets.First(bp => bp.name == "BobombMaster");
                contentPack.masterPrefabs.Add(new GameObject[] { bobombMaster });
                if (SM64BBF.RegigigasCompat.enabled)
                {
                    var kingBobombMaster2 = assets.First(bp => bp.name == "KingBobomb2Master");
                    contentPack.masterPrefabs.Add(new GameObject[] { kingBobombMaster2 });
                    //var kingBobombMaster = assets.First(bp => bp.name == "KingBobombMaster");
                    //contentPack.masterPrefabs.Add(new GameObject[] { kingBobombMaster });
                    //Log.Debug("added and setup RegigigasKingBobombMaster");
                }
                //Log.Debug("loaded masterPrefabs");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SkillDef[]>)((assets) =>
            {
                contentPack.skillDefs.Add(assets);
                if (SM64BBF.RegigigasCompat.enabled)
                {
                    RegigigasCompat.SetupPrimarySkill(assets.First(sd => (sd as ScriptableObject).name == "KingBobombGrab"));
                    RegigigasCompat.SetupSecondarySkill(assets.First(sd => (sd as ScriptableObject).name == "KingBobombEarthquake"));
                    RegigigasCompat.SetupUtilitySkill(assets.First(sd => (sd as ScriptableObject).name == "KingBobombSlam"));
                    RegigigasCompat.SetupSpecialSkill(assets.First(sd => (sd as ScriptableObject).name == "KingBobombRevenge"));
                    ////Log.Debug("added and setup KingBobomb Skills");
                }
                //Log.Debug("loaded skillDefs");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<SkillFamily[]>)((assets) =>
            {
                contentPack.skillFamilies.Add(assets);
                //Log.Debug("loaded skillFamilies");
            }));

            yield return LoadAllAssetsAsync(_assetsAssetBundle, progress, (Action<ItemDisplayRuleSet[]>)((assets) =>
            {
                var idrsBobomb = assets.First(idrs => idrs.name == "idrsBobomb");
                SetupBobombItemDisplays(ref idrsBobomb);
                //Log.Debug("setup idrs");
            }));

            contentPack.entityStateTypes.Add(new Type[] { typeof(SM64BBF.States.StarManState), typeof(States.BobombAcquireTargetState), typeof(States.BobombDeathState), typeof(States.BobombExplodeState), typeof(States.BobombSpawnState), typeof(States.BobombSuicideDeathState) });

            var bossDroplet = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/BossOrb.prefab");
            while (!bossDroplet.IsDone)
            {
                yield return null;
            }

            MiscPickups.Starman.dropletDisplayPrefab = bossDroplet.Result;
            MiscPickups.Coin.dropletDisplayPrefab = bossDroplet.Result;

            WHBazaarSeer = StageRegistration.MakeBazaarSeerMaterial(SM64BBFPreviewSprite.texture);
            WHSceneDef.previewTexture = SM64BBFPreviewSprite.texture;
            WHSceneDef.portalMaterial = WHBazaarSeer;

            var dioramaPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/golemplains/GolemplainsDioramaDisplay.prefab");
            while (!dioramaPrefab.IsDone)
            {
                yield return null;
            }
            WHSceneDef.dioramaPrefab = dioramaPrefab.Result;

            StageRegistration.RegisterSceneDefToLoop(WHSceneDef);
            //Log.Debug(WHSceneDef.destinationsGroup);
        }

        private static void SetupBobombItemDisplays(ref ItemDisplayRuleSet itemDisplayRuleSet)
        {
            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(-0.15365F, 0.21462F, -0.27426F),
                localAngles = new Vector3(354.7525F, 302.5303F, 7.12234F),
                localScale = new Vector3(0.44488F, 0.44488F, 0.44488F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(-0.12521F, 0.3066F, 0.3031F),
                localAngles = new Vector3(354.7525F, 231.6184F, 351.6044F),
                localScale = new Vector3(-0.44F, 0.44F, 0.44F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteFire/EliteFireEquipment.asset").WaitForCompletion(),
                displayRuleGroup = displayRuleGroupFire,
            });
            #endregion

            #region HauntedElite
            var displayRuleGroupHaunted = new DisplayRuleGroup();
            displayRuleGroupHaunted.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/DisplayEliteStealthCrown.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.02002F, 0.36085F, 0.01142F),
                localAngles = new Vector3(84.62356F, 329.7962F, 243.3615F),
                localScale = new Vector3(0.20258F, 0.20258F, 0.20258F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup 
            { 
                displayRuleGroup = displayRuleGroupHaunted, 
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteHaunted/EliteHauntedEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region IceElite
            var displayRuleGroupIce = new DisplayRuleGroup();
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteIce/DisplayEliteIceCrown.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(0.05636F, 0.20679F, 0.06288F),
                localAngles = new Vector3(283.3743F, 102.7717F, 166.9378F),
                localScale = new Vector3(0.13275F, 0.13275F, 0.13275F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupIce,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteIce/EliteIceEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region LightningElite
            var displayRuleGroupLightning = new DisplayRuleGroup();
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.47095F, 0.3003F, -0.02387F),
                localAngles = new Vector3(355.5493F, 269.5843F, 12.25919F),
                localScale = new Vector3(0.65886F, 0.65886F, 0.65886F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.41474F, 0.41538F, -0.02514F),
                localAngles = new Vector3(323.8193F, 261.7038F, 7.48606F),
                localScale = new Vector3(0.44488F, 0.44488F, 0.44488F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLightning,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteLightning/EliteLightningEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region LunarElite
            var displayRuleGroupLunar = new DisplayRuleGroup();
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(0.64709F, -0.88823F, 0.02083F),
                localAngles = new Vector3(11.76858F, 84.12247F, 3.64591F),
                localScale = new Vector3(0.44488F, 0.44488F, 0.44488F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar,Eye.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.05048F, -0.05632F, -0.00397F),
                localAngles = new Vector3(81.85596F, 299.8571F, 309.0397F),
                localScale = new Vector3(1.02235F, 1.02235F, 1.02235F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLunar,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteLunar/EliteLunarEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region PoisonElite
            var displayRuleGroupPoison = new DisplayRuleGroup();
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(0.05243F, 0.2016F, -0.02218F),
                localAngles = new Vector3(280.5813F, 114.8507F, 252.4087F),
                localScale = new Vector3(0.25238F, 0.25238F, 0.25238F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupPoison,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/ElitePoison/ElitePoisonEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region EliteEarth
            var displayRuleGroupEarth = new DisplayRuleGroup();
            displayRuleGroupEarth.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/DisplayEliteMendingAntlers.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.12323F, 0.22929F, 0.00569F),
                localAngles = new Vector3(13.38845F, 89.25855F, 356.7623F),
                localScale = new Vector3(2.60453F, 2.60453F, 2.60453F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupEarth,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteEarth/EliteEarthEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region VoidElite
            var displayRuleGroupVoid = new DisplayRuleGroup();
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/DisplayAffixVoid.prefab").WaitForCompletion(),
                childName = "Eyes",
                localPos = new Vector3(-0.01601F, 0.35024F, -0.22315F),
                localAngles = new Vector3(20.17711F, 258.0909F, 2.55114F),
                localScale = new Vector3(0.36462F, 0.36462F, 0.36462F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/DisplayAffixVoid.prefab").WaitForCompletion(),
                childName = "Eyes",
                localPos = new Vector3(-0.1112F, 0.35393F, 0.20253F),
                localAngles = new Vector3(20.17711F, 258.0909F, 2.55114F),
                localScale = new Vector3(0.36462F, 0.36462F, 0.36462F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupVoid,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteVoid/EliteVoidEquipment.asset").WaitForCompletion()
            });
            #endregion

            //#region IceElite
            //var displayRuleGroup = new DisplayRuleGroup();
            //displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            //{
            //    ruleType = ItemDisplayRuleType.ParentedPrefab,
            //    followerPrefab = Addressables.LoadAssetAsync<GameObject>("").WaitForCompletion(),
            //    childName = "Head",
            //    localPos = new Vector3(-0.15365F, 0.21462F, -0.27426F),
            //    localAngles = new Vector3(354.7525F, 302.5303F, 7.12234F),
            //    localScale = new Vector3(0.44488F, 0.44488F, 0.44488F),
            //    limbMask = LimbFlags.None
            //});

            //ArrayUtils.ArrayAppend(ref itemDisplayRuleSet.keyAssetRuleGroups, new KeyAssetRuleGroup
            //{
            //    displayRuleGroup = displayRuleGroupHaunted,
            //    keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("").WaitForCompletion()
            //});
            //#endregion
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }

        private static void SetupMusic()
        {
            var mainCustomTrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            mainCustomTrack.cachedName = "SM64BBFCustomMainMusic";
            mainCustomTrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();

            var cstate1 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate1.GroupId = 1405583565U; // gathered from the MOD's Init bank txt file
            cstate1.StateId = 693170834U; // Maxwell's theme
            //cstate1.StateId = 145640315U; // Maxwell's theme

            mainCustomTrack.CustomStates.Add(cstate1);
            var cstate2 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            cstate2.GroupId = 792781730U; // gathered from the GAME's Init bank txt file
            cstate2.StateId = 89505537U; // gathered from the GAME's Init bank txt file
            mainCustomTrack.CustomStates.Add(cstate2);

            WHSceneDef.mainTrack = mainCustomTrack;

            //var bossCustomTrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            //bossCustomTrack.cachedName = "SM64BBFCustomBossMusic";
            //bossCustomTrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();

            //var cstate11 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            //cstate11.GroupId = 487602916U; // gathered from the MOD's Init bank txt file
            //cstate11.StateId = 3699353111U; // DiesIrae

            //bossCustomTrack.CustomStates.Add(cstate11);
            //var cstate12 = new SoundAPI.Music.CustomMusicTrackDef.CustomState();
            //cstate12.GroupId = 792781730U; // gathered from the GAME's Init bank txt file
            //cstate12.StateId = 580146960U; // gathered from the GAME's Init bank txt file
            //bossCustomTrack.CustomStates.Add(cstate12);

            //WHSceneDef.bossTrack = bossCustomTrack;
        }

        internal static void LoadSoundBanks(string soundbanksFolderPath)
        {
            var akResult = AkSoundEngine.AddBasePath(soundbanksFolderPath);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank base path : {soundbanksFolderPath}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {soundbanksFolderPath} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(InitSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {InitSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {InitSoundBankFileName} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(MusicSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {MusicSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {MusicSoundBankFileName} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(SoundsSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {SoundsSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {SoundsSoundBankFileName} " +
                    $"Error code : {akResult}");
            }
        }
    }
}