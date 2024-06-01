using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace SM64BBF.States
{
    public class BobombAcquireTargetState : BaseState
    {
        public static float baseDuration = 1f;

        private static float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Body", "EmoteSurprise", "Surprise.playbackRate", duration);
            //EntitySoundManager.EmitSoundServer((AkEventIdArg)"SM64_BBF_Play_Bobomb_Aggro", gameObject);
            Util.PlayAttackSpeedSound("SM64_BBF_Play_Bobomb_Aggro", gameObject, attackSpeedStat);
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
                if(NetworkServer.active)
                {
                    characterBody.AddBuff(SM64BBFContent.Buffs.BobombArmor);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
