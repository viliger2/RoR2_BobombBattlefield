using BepInEx;
using RegigigasMod.Modules;
using RoR2;
using RoR2.ContentManagement;
using SM64BBF.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SM64BBF
{
    [BepInPlugin(GUID, "SM64BBF", Version)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.StageRegistration.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("com.rob.RegigigasMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class SM64BBFPlugin : BaseUnityPlugin
    {
        public const string Version = "1.0.0";
        public const string GUID = "com.Viliger.SM64BBF";

        public static SM64BBFPlugin instance;

        //public static GameObject wwiseSM64ObjectInstance;

        //public static AssetBundle SM64AudioManagerBundle;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            RegisterHooks();

            RegisterSounds();

#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
        }

        private void RegisterHooks()
        {
            On.RoR2.MusicController.Start += MusicController_Start;
            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;
            Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            CharacterBody.onBodyInventoryChangedGlobal += SM64BBF.Items.MarioOneUpItemBehavior.CharacterBody_onBodyInventoryChangedGlobal;
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            //throw new NotImplementedException();
            if (sender.HasBuff(SM64BBF.SM64BBFContent.Buffs.BobombArmor))
            {
                args.armorAdd += 100f;
            }
        }

        private static void RegisterSounds()
        {
            RegisterNetworkSound("SM64_BBF_Play_Coin");
            RegisterNetworkSound("SM64_BBF_Stop_StarmanComes");
            RegisterNetworkSound("SM64_BBF_StarmanKills");
            RegisterNetworkSound("SM64_BBF_Play_Star");
            RegisterNetworkSound("SM64_BBF_Stop_RollingStone");
            RegisterNetworkSound("SM64_BBF_Play_StarmanComes");
            RegisterNetworkSound("SM64_BBF_Play_RollingStone");
            RegisterNetworkSound("SM64_BBF_solonggaybowser");
            RegisterNetworkSound("SM64_BBF_Play_OneUp");
            RegisterNetworkSound("SM64_BBF_Play_Bobomb_Aggro");
            RegisterNetworkSound("SM64_BBF_Play_Bobomb_Fuse");
            RegisterNetworkSound("SM64_BBF_Stop_Bobomb_Fuse");
            RegisterNetworkSound("SM64_BBF_Play_Bobomb_Death");
            RegisterNetworkSound("SM64_BBF_Play_Shake_Tree");

        }

        private void MusicController_Start(On.RoR2.MusicController.orig_Start orig, MusicController self)
        {
            orig(self);
            AkSoundEngine.PostEvent("SM64_BBF_Play_Music_System", self.gameObject);
        }

        private void Destroy()
        {
            Language.collectLanguageRootFolders -= CollectLanguageRootFolders;
        }
      
        private void GiveToRoR2OurContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new Content.ContentProvider());
        }
    
        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
        }

        public static bool RegisterNetworkSound(string eventName)
        {
            RoR2.NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<RoR2.NetworkSoundEventDef>();
            networkSoundEventDef.eventName = eventName;

            return R2API.ContentAddition.AddNetworkSoundEventDef(networkSoundEventDef);
        }
    }
}