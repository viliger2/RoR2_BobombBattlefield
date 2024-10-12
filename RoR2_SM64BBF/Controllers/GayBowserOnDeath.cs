using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace SM64BBF.Controllers
{
    public class GayBowserOnDeath : MonoBehaviour
    {

        private void OnEnable()
        {
            if (!NetworkServer.active)
            {
                this.enabled = false;
                return;
            }
            GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath;
            RoR2.TeleporterInteraction.onTeleporterFinishGlobal += TeleporterInteraction_onTeleporterFinishGlobal;
        }

        private void OnDisable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath;
            RoR2.TeleporterInteraction.onTeleporterFinishGlobal -= TeleporterInteraction_onTeleporterFinishGlobal;
        }

        private void TeleporterInteraction_onTeleporterFinishGlobal(TeleporterInteraction teleporterInteraction)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_ThankYou", teleporterInteraction.gameObject);
        }

        private void OnCharacterDeath(DamageReport damageReport)
        {
            if (damageReport.victimBody.isPlayerControlled)
            {
                if (damageReport.attacker)
                {
                    EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_solonggaybowser", damageReport.attacker);
                }
                else if (damageReport.victimMaster)
                {
                    EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_solonggaybowser", damageReport.victimMaster.gameObject);
                }
            }
        }
    }
}
