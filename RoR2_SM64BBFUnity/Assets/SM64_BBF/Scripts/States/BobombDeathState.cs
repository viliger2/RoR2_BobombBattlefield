using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM64BBF.States
{
    public class BobombDeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Stop_Bobomb_Fuse", gameObject);
        }
    }
}
