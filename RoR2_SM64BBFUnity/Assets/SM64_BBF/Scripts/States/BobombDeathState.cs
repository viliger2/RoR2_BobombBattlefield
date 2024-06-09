using EntityStates;
using RoR2.Audio;

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
