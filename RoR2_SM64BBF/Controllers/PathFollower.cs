using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace SM64BBF.Controllers
{
    public class PathFollower : MonoBehaviour
    {
        internal Transform[] path;
        internal float speed = 5f;
        internal float reachDist = 1f;

        internal GameObject deathEffectPrefab;

        private int currentPoint = 0;

        // Start is called before the first frame update
        void Start()
        {
            Util.PlaySound("SM64_BBF_Play_RollingStone", gameObject);
        }

        void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            float dist = Vector3.Distance(path[currentPoint].position, transform.position);

            transform.position = Vector3.MoveTowards(transform.position, path[currentPoint].position, Time.deltaTime * speed);

            if (dist <= reachDist)
            {
                currentPoint++;
            }

            if (currentPoint >= path.Length)
            {
                EffectManager.SimpleMuzzleFlash(deathEffectPrefab, gameObject, "SmokeBomb", true);
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            Util.PlaySound("SM64_BBF_Stop_RollingStone", gameObject);
        }
    }
}
