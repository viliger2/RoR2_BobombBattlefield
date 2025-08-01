using EntityStates;
using RoR2;
using RoR2.Audio;

namespace SM64BBF.States
{
    public class BobombDeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("SM64_BBF_Stop_Bobomb_Fuse", gameObject);
        }
    }
}
