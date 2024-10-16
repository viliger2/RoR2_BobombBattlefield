﻿using RoR2;
using UnityEngine;

namespace SM64BBF.PickUpDefs
{
    [CreateAssetMenu(menuName = "SM64BBF/MiscPickupDefs/CoinPickupDef")]
    public class CoinPickupDef : MiscPickupDef
    {
        private static float healValue = 0.125f;

        public override void GrantPickup(ref PickupDef.GrantContext context)
        {
            context.body.healthComponent.HealFraction(healValue, default(ProcChainMask));
            context.shouldDestroy = true;
            context.shouldNotify = false;
        }

        public override string GetInternalName()
        {
            return "MiscPickupIndex.Coin";
        }
    }
}
