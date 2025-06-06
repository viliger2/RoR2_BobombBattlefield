using BepInEx;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.ContentManagement;
using SM64BBF.Items;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SM64BBF
{
    [BepInPlugin(GUID, "SM64BBF", Version)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("JaceDaDorito.LocationsOfPrecipitation")]
    [BepInDependency("com.rob.RegigigasMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class SM64BBFPlugin : BaseUnityPlugin
    {
        public const string Version = "1.1.1";
        public const string GUID = "com.Viliger.SM64BBF";

        public static SM64BBFPlugin instance;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            SM64BBF.Config.PopulateConfig(Config);

            RegisterHooks();

            if (!SM64BBF.RegigigasCompat.enabled)
            {
                DirectorAPI.Helpers.RemoveExistingMonster("cscKingBobomb2");
            }

#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
        }

        private void RegisterHooks()
        {
            On.RoR2.MusicController.StartIntroMusic += MusicController_StartIntroMusic;
            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;
            Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            MarioOneUpItemBehavior.CharacterBody_onBodyInventoryChangedGlobal(body);
            if (NetworkServer.active)
            {
                if(body && body.inventory)
                {
                    body.AddItemBehavior<RoyalCrownItemBehavior>(body.inventory.GetItemCount(SM64BBFContent.Items.RoyalCrown));
                }
            }
        }

        private void MusicController_StartIntroMusic(On.RoR2.MusicController.orig_StartIntroMusic orig, MusicController self)
        {
            orig(self);
            AkSoundEngine.PostEvent("SM64_BBF_Play_Music_System", self.gameObject);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            //throw new NotImplementedException();
            if (sender.HasBuff(SM64BBF.SM64BBFContent.Buffs.BobombArmor))
            {
                args.armorAdd += 100f;
            }
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