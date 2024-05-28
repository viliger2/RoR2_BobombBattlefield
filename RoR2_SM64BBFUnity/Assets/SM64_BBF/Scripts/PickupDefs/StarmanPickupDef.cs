using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            context.shouldNotify = true;
        }

        public override string GetInternalName()
        {
            return "MistPickupIndex.Starman";
        }
    }
}
