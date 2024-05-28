using EntityStates;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace SM64BBF.States
{
    public class BobombSuicideDeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Stop_Bobomb_Fuse", gameObject);
            healthComponent.Suicide(); // this ensures OnDeath events like elite things
            DestroyModel();
            if (NetworkServer.active)
            {
                DestroyBodyAsapServer();
            }
        }
    }
}
