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

                    spawnedMaster.inventory.CopyEquipmentFrom(damageReport.victimMaster.inventory, false);

                    float eliteHealthBoost = 0f;
                    float eliteDamageBoost = 0f;

                    var victimInventory = damageReport.victimMaster.inventory;

                    for (uint i = 0; i < victimInventory.GetEquipmentSlotCount(); i++)
                    {
                        for(uint j = 0; j < victimInventory.GetEquipmentSetCount(i); j++)
                        {
                            var equipment = victimInventory.GetEquipment(i, j);
                            if (equipment.equipmentDef && equipment.equipmentDef.passiveBuffDef && equipment.equipmentDef.passiveBuffDef.isElite) // checking for passive buff def so 
                            {
                                eliteDef = equipment.equipmentDef.passiveBuffDef.eliteDef;

                                eliteHealthBoost += eliteDef.healthBoostCoefficient;
                                eliteDamageBoost += eliteDef.damageBoostCoefficient;
                            }
                        }
                    }

                    var healthBoost = eliteHealthBoost + (base.body.inventory.GetItemCountEffective(SM64BBF.SM64BBFContent.Items.RoyalCrown) - 1);
                    var damageBoost = eliteDamageBoost;

                    spawnedMaster.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, Mathf.RoundToInt((healthBoost) * 10f));
                    spawnedMaster.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((damageBoost) * 10f));
                    spawnedMaster.inventory.GiveItemPermanent(RoR2Content.Items.HealthDecay, 30);

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
