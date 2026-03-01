using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.MasterCatalog;

namespace SM64BBF.Artifacts
{
    public class BobombOnDeathManager
    {
        public static MasterIndex BobombMasterIndex;

        [SystemInitializer(new Type[] { typeof(ArtifactCatalog), typeof(MasterCatalog) })]
        private static void Init()
        {
            if (!SM64BBFPlugin.isLoaded)
            {
                return;
            }

            BobombMasterIndex = MasterCatalog.FindMasterIndex(SM64BBFContent.BobombMaster);

            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        private static void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (NetworkServer.active && artifactDef == SM64BBFContent.BobombOnDeath)
            {
                GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            }
        }

        private static void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == SM64BBFContent.BobombOnDeath)
            {
                GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!NetworkServer.active || damageReport == null)
            {
                return;
            }

            GameObject gameObject = null;
            if ((bool)damageReport.victim)
            {
                gameObject = damageReport.victim.gameObject;
            }

            CharacterBody victimBody = damageReport.victimBody;
            CharacterMaster victimMaster = damageReport.victimMaster;
            TeamIndex teamIndex = damageReport.victimTeamIndex;

            if (victimBody && victimMaster)
            {

                if (teamIndex == TeamIndex.Monster && victimMaster.masterIndex != BobombMasterIndex)
                {
                    Vector3 position3 = victimBody.corePosition;

                    MasterSummon masterSummon = new MasterSummon();
                    masterSummon.position = position3;
                    masterSummon.ignoreTeamMemberLimit = true;
                    masterSummon.masterPrefab = SM64BBFContent.BobombMaster;
                    masterSummon.summonerBodyObject = gameObject;
                    masterSummon.rotation = Quaternion.identity;
                    masterSummon.Perform();
                }
            }
        }
    }
}
