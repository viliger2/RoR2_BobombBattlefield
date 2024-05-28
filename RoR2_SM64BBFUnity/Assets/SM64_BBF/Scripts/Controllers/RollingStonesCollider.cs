using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SM64BBF.Controllers
{
    public class RollingStonesCollider : MonoBehaviour
    {
        public Collider ourCollider;

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == 0)
            {
                if (NetworkServer.active)
                {
                    var contact = collision.GetContact(0);

                    BlastAttack blastAttack2 = new BlastAttack();
                    blastAttack2.radius = contact.separation + 2f;
                    blastAttack2.procCoefficient = 0f;
                    blastAttack2.position = contact.point;
                    blastAttack2.attacker = null;
                    blastAttack2.crit = false;
                    blastAttack2.baseDamage = 25f;
                    blastAttack2.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack2.baseForce = 8000f;
                    blastAttack2.teamIndex = TeamIndex.Neutral;
                    blastAttack2.damageType = DamageType.BypassArmor;
                    blastAttack2.attackerFiltering = AttackerFiltering.Default;
                    blastAttack2.canRejectForce = false;
                    blastAttack2.Fire();
                }
            }
        }
    }
}
