using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SM64BBF.States
{
    public class BobombSpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            if (spawnEffect)
            {
                EffectManager.SimpleMuzzleFlash(spawnEffect, gameObject, "Chest", true);
            }
        }
    }
}
