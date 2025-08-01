using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.GenericPickupController;

namespace SM64BBF.PickUpDefs
{
    [CreateAssetMenu(menuName = "SM64BBF/MiscPickupDefs/StarmanPickupDef")]
    public class StarmanPickupDef : MiscPickupDef
    {
        public override void GrantPickup(ref PickupDef.GrantContext context)
        {
            context.shouldDestroy = true;
            context.shouldNotify = true; // results in redlogging, but its fine
        }

        public override string GetInternalName()
        {
            return "MistPickupIndex.Starman";
        }

        public static void GenericPickupController_HandlePickupMessage(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if(c.TryGotoNext(MoveType.After,
                x => x.MatchLdstr(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchCallvirt(out _),
                x => x.MatchCall(out _),
                x => x.MatchPop()))
            {
                c.Emit(OpCodes.Ldloc, 2);
                c.Emit(OpCodes.Ldloc, 1);
                c.EmitDelegate<Action<RoR2.PickupIndex, GameObject>>(DoTheThingWithTheThing);
            } else
            {
                Log.Error($"GenericPickupController_HandlePickupMessage ILHook failed");
            }
        }

        private static void DoTheThingWithTheThing(RoR2.PickupIndex pickupIndex, GameObject masterGameObject)
        {
            if (pickupIndex != PickupCatalog.FindPickupIndex(SM64BBFContent.MiscPickups.Starman.miscPickupIndex))
            {
                return;
            }
            if (!Util.HasEffectiveAuthority(masterGameObject))
            {
                return;
            }

            var master = masterGameObject.GetComponent<CharacterMaster>();
            if (!master)
            {
                return;
            }

            var bodyObject = master.GetBodyObject();
            if (!bodyObject)
            {
                return;
            }

            EntityStateMachine.FindByCustomName(bodyObject, "Body")?.SetState(RoR2.EntityStateCatalog.InstantiateState(typeof(SM64BBF.States.StarManState)));
        }
    }
}
