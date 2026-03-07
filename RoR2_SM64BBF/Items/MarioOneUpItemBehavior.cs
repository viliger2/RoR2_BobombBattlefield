using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.CharacterMaster;

namespace SM64BBF.Items
{
    public class MarioOneUpItemBehavior : MonoBehaviour
    {
        public CharacterMaster master;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Stage.onServerStageBegin += Stage_onServerStageBegin;
        }

        public void OnDestroy()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Stage.onServerStageBegin -= Stage_onServerStageBegin;
        }

        private void Stage_onServerStageBegin(Stage obj)
        {
            if (master)
            {
                while (master.inventory.GetItemCountPermanent(SM64BBFContent.Items.MarioOneUp) > 0)
                {
                    master.inventory.RemoveItemPermanent(SM64BBFContent.Items.MarioOneUp);
                }
            }
        }

        public static bool CharacterMaster_TryReviveOnBodyDeath(On.RoR2.CharacterMaster.orig_TryReviveOnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            Inventory.ItemTransformation itemTransformation = default(Inventory.ItemTransformation);
            itemTransformation.originalItemIndex = SM64BBFContent.Items.MarioOneUp.itemIndex;
            itemTransformation.newItemIndex = ItemIndex.None;
            itemTransformation.transformationType = (ItemTransformationTypeIndex)0;
            if (itemTransformation.TryTake(self.inventory, out var result))
            {
                ExtraLifeServerBehavior extraLifeServerBehavior = self.gameObject.AddComponent<ExtraLifeServerBehavior>();
                extraLifeServerBehavior.pendingTransformation = result;
                extraLifeServerBehavior.completionTime = Run.FixedTimeStamp.now + 2f;
                extraLifeServerBehavior.consumedItemIndex = ItemIndex.None;
                extraLifeServerBehavior.completionCallback = self.RespawnExtraLife;
                extraLifeServerBehavior.soundTime = extraLifeServerBehavior.completionTime - 1f;
                extraLifeServerBehavior.soundCallback = self.PlayExtraLifeSFX;
                return true;
            }

            return orig(self, body);
        }

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (body && body.inventory && body.master)
            {
                var itemCount = body.inventory.GetItemCountPermanent(SM64BBFContent.Items.MarioOneUp);
                var component = body.master.gameObject.GetComponent<MarioOneUpItemBehavior>();
                if (!component && itemCount > 0)
                {
                    var component2 = body.master.gameObject.AddComponent<MarioOneUpItemBehavior>();
                    component2.master = body.master;
                }
                else if (itemCount == 0 && component)
                {
                    UnityEngine.Object.Destroy(component);
                }
            }
        }

    }
}
