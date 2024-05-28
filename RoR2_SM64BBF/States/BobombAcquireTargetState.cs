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
    public class BobombAcquireTargetState : BaseState
    {
        private static float duration = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body", "EmoteSurprise", "Surprise.playbackRate", duration);
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Play_Bobomb_Aggro", gameObject);
            var setStateOnHurt = gameObject.GetComponent<SetStateOnHurt>();
            if (setStateOnHurt)
            {
                setStateOnHurt.canBeStunned = false;
                setStateOnHurt.canBeHitStunned = false;
                setStateOnHurt.canBeFrozen = false;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge > duration)
            {
                outer.SetNextState(new BobombExplodeState());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
