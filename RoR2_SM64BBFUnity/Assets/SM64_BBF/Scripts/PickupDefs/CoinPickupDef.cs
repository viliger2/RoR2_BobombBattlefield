using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SM64BBF.PickUpDefs
{
    [CreateAssetMenu(menuName = "SM64BBF/MiscPickupDefs/CoinPickupDef")]
    public class CoinPickupDef : MiscPickupDef
    {
        private static float healValue = 12.5f; 

        public override void GrantPickup(ref PickupDef.GrantContext context)
        {
            context.body.healthComponent.HealFraction(healValue, default(ProcChainMask));
            context.shouldDestroy = true;
            context.shouldNotify = false;
        }

        public override string GetInternalName()
        {
            return "MistPickupIndex.Coin";
        }
    }
}
