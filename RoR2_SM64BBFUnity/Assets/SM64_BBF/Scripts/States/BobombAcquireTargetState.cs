using EntityStates;
using RoR2;
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
            if (base.fixedAge > duration)
            {
                outer.SetNextState(new BobombExplodeState());
                if (NetworkServer.active)
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
