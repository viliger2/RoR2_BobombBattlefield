using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.CharacterBody;

namespace SM64BBF.Items
{
    public class RoyalCrownItemBehavior : ItemBehavior, IOnKilledOtherServerReceiver
    {
        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (!damageReport.victimBody || !damageReport.victimMaster || !damageReport.victimMaster.inventory)
            {
                return;
            }

            if (!damageReport.victimIsElite)
            {
                return;
            }

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(SM64BBFContent.CharacterSpawnCards.cscBobomb, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                position = damageReport.victimBody.transform.position
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = base.gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (SpawnCard.SpawnResult spawnResult) =>
            {
                if (spawnResult.success && spawnResult.spawnedInstance)
                {
                    var spawnedMaster = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();

                    var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                    if (aiownership)
                    {
                        aiownership.ownerMaster = this.body.master;
                    }

                    EliteDef eliteDef = null;

                    for(uint i = 0; i < damageReport.victimMaster.inventory.GetEquipmentSlotCount(); i++)
                    {
                        var equipment = damageReport.victimMaster.inventory.GetEquipment(i);
                        if(equipment.equipmentDef && equipment.equipmentDef.passiveBuffDef && equipment.equipmentDef.passiveBuffDef.isElite) // checking for passive buff def so 
                        {
                            eliteDef = equipment.equipmentDef.passiveBuffDef.eliteDef;
                            break;
                        }
                    }

                    var healthBoost = eliteDef?.healthBoostCoefficient ?? 1f + (base.body.inventory.GetItemCount(SM64BBF.SM64BBFContent.Items.RoyalCrown) - 1);
                    var damageBoost = eliteDef?.damageBoostCoefficient ?? 1f;
                    var equipmentIndex = eliteDef?.eliteEquipmentDef?.equipmentIndex ?? EquipmentIndex.None;
                    if(equipmentIndex != EquipmentIndex.None)
                    {
                        spawnedMaster.inventory.SetEquipmentIndex(equipmentIndex);
                    }

                    spawnedMaster.inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((healthBoost - 1f) * 10f));
                    spawnedMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((damageBoost - 1f) * 10f));
                    spawnedMaster.inventory.GiveItem(RoR2Content.Items.HealthDecay, 30);

                    var baseAI = spawnResult.spawnedInstance.GetComponent<BaseAI>();
                    if (baseAI && baseAI.body)
                    {
                        baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                }
            };
            DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
        }
    }
}
