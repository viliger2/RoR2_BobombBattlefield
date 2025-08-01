using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine.Networking;

namespace SM64BBF.States
{
    public class BobombSuicideDeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("SM64_BBF_Stop_Bobomb_Fuse", gameObject);
            if (NetworkServer.active)
            {
                healthComponent.Suicide(); // this ensures OnDeath events like elite things
            }
            DestroyModel();
            if (NetworkServer.active)
            {
                DestroyBodyAsapServer();
            }
        }
    }
}
