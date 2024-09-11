using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;
using Vector3 = UnityEngine.Vector3;

namespace SM64BBF.Interactables
{
    public class MarioTreeInteractableManager : NetworkBehaviour, IInteractable, IDisplayNameProvider
    {
        [SyncVar]
        public string displayNameToken;

        [SyncVar]
        public string contextToken;

        [SyncVar]
        public bool available;

        private Xoroshiro128Plus rng;

        private Transform itemSpawnPoint;

        private void Start()
        {
            if (NetworkServer.active)
            {
                rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
                //toothHealPack = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Tooth/HealPack.prefab").WaitForCompletion();
                itemSpawnPoint = gameObject.transform.Find("Tree/ItemSpawnPoint");
            }
        }

        private void OnEnable()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            available = false;
        }

        private void OnDisable()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return Language.GetString(contextToken);
        }

        public string GetDisplayName()
        {
            return Language.GetString(displayNameToken);
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            if (!available)
            {
                return Interactability.Disabled;
            }

            return Interactability.Available;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Play_Shake_Tree", gameObject);
            Invoke("DropStuff", 0.5f);

            available = false;
        }

        private void DropStuff()
        {

#if DEBUG
            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(SM64BBF.SM64BBFContent.MiscPickups.Starman.miscPickupIndex);
            PickupDropletController.CreatePickupDroplet(pickupIndex, itemSpawnPoint.position, Vector3.up * 5f + transform.forward * 3f);
            PickupIndex pickupIndex2 = PickupCatalog.FindPickupIndex(SM64BBFContent.Items.MarioOneUp.itemIndex);
            PickupDropletController.CreatePickupDroplet(pickupIndex2, itemSpawnPoint.position, Vector3.up * 5f + transform.forward * 3f);
            PickupIndex pickupIndex3 = PickupCatalog.FindPickupIndex(SM64BBFContent.MiscPickups.Coin.miscPickupIndex);
            PickupDropletController.CreatePickupDroplet(pickupIndex3, itemSpawnPoint.position, Vector3.up * 5f + transform.forward * 3f);
#else
            WeightedSelection<(string, PickupIndex)> selection = new WeightedSelection<(string, PickupIndex)>();
            selection.AddChoice(("SM64_BBF_Play_Coin", PickupCatalog.FindPickupIndex(SM64BBFContent.MiscPickups.Coin.miscPickupIndex)), Config.TreeInteractable.CoinWeight.Value);
            selection.AddChoice(("SM64_BBF_Play_OneUp", PickupCatalog.FindPickupIndex(SM64BBFContent.Items.MarioOneUp.itemIndex)), Config.TreeInteractable.OneUpWeight.Value);
            selection.AddChoice(("SM64_BBF_Play_Star", PickupCatalog.FindPickupIndex(SM64BBF.SM64BBFContent.MiscPickups.Starman.miscPickupIndex)), Config.TreeInteractable.StarmanWeight.Value);
            selection.AddChoice(default((string, PickupIndex)), Config.TreeInteractable.NothingWeight.Value);

            var pickupIndex = selection.Evaluate(Run.instance.treasureRng.nextNormalizedFloat);
            if(pickupIndex != default((string, PickupIndex)))
            {
                EntitySoundManager.EmitSoundServer((AkEventIdArg)pickupIndex.Item1, gameObject);
                PickupDropletController.CreatePickupDroplet(pickupIndex.Item2, itemSpawnPoint.position, Vector3.up * 5f + transform.forward * 3f);
            }
#endif
        }

        public bool ShouldIgnoreSpherecastForInteractibility([NotNull] Interactor activator)
        {
            return false;
        }

        public bool ShouldShowOnScanner()
        {
            return false;
        }

        public bool ShouldProximityHighlight()
        {
            return true;
        }
    }
}
