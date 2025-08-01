using RoR2;
using UnityEngine;

namespace SM64BBF.PickUpDefs
{
    [CreateAssetMenu(menuName = "SM64BBF/MiscPickupDefs/CoinPickupDef")]
    public class CoinPickupDef : MiscPickupDef
    {
        private static float healValue = 0.125f;

        private static int moneyReward = 25;

        public override void GrantPickup(ref PickupDef.GrantContext context)
        {
            context.body.healthComponent.HealFraction(healValue, default(ProcChainMask));
            context.body.master.GiveMoney((uint)Run.instance.GetDifficultyScaledCost(moneyReward));
            context.shouldDestroy = true;
            context.shouldNotify = false;
        }

        public override string GetInternalName()
        {
            return "MiscPickupIndex.Coin";
        }
    }
}
