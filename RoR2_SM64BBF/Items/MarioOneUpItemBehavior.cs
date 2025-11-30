using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace SM64BBF.Items
{
    // TODO: Rewrite this garbage to IL hook for master
    public class MarioOneUpItemBehavior : MonoBehaviour
    {
        public CharacterMaster master;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
            Stage.onServerStageBegin += Stage_onServerStageBegin;
        }

        public void OnDestroy()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            On.RoR2.CharacterMaster.OnBodyDeath -= CharacterMaster_OnBodyDeath;
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

        private void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            if (NetworkServer.active &&
                self && self == master && self.inventory && self.inventory.GetItemCountPermanent(SM64BBFContent.Items.MarioOneUp) > 0)
            {
                self.lostBodyToDeath = true;
                self.deathFootPosition = body.footPosition;
                BaseAI[] array = self.aiComponents;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].OnBodyDeath(body);
                }
                if ((bool)self.playerCharacterMasterController)
                {
                    self.playerCharacterMasterController.OnBodyDeath();
                }
                Invoke("Respawn", 2f);
                Invoke("PlayExtraLifeSFX", 1f);
                self.ResetLifeStopwatch();
                self.onBodyDeath?.Invoke();
            }
            else
            {
                orig(self, body);
            }
        }

        public void Respawn()
        {
            master.RespawnExtraLife();
            master.inventory.RemoveItemPermanent(SM64BBFContent.Items.MarioOneUp);
        }

        public void PlayExtraLifeSFX()
        {
            master.PlayExtraLifeSFX();
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
