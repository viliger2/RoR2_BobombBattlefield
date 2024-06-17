using RoR2;
using UnityEngine;

namespace SM64BBF.PickUpDefs
{
    [CreateAssetMenu(menuName = "SM64BBF/MiscPickupDefs/StarmanPickupDef")]
    public class StarmanPickupDef : MiscPickupDef
    {
        public override void GrantPickup(ref PickupDef.GrantContext context)
        {
            EntityStateMachine.FindByCustomName(context.body.gameObject, "Body")?.SetState(RoR2.EntityStateCatalog.InstantiateState(typeof(SM64BBF.States.StarManState)));
            context.shouldDestroy = true;
            context.shouldNotify = true; // results in redlogging, but its fine
        }

        public override string GetInternalName()
        {
            return "MistPickupIndex.Starman";
        }
    }
}
