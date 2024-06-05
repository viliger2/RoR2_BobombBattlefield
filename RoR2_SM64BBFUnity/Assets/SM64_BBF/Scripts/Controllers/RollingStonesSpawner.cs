using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SM64BBF.Controllers
{
    public class RollingStonesSpawner : MonoBehaviour
    {
        public GameObject rollingStonePrefab;

        public Transform[] path;

        public float spawnTimer = 4f;

        public float speed = 25f;

        private float lastStoneTimer;

        private GameObject smokeBombPrefab;

        private void Start()
        {
            smokeBombPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            lastStoneTimer += Time.fixedDeltaTime;
            if(lastStoneTimer > spawnTimer)
            {
                var newObject = UnityEngine.Object.Instantiate(rollingStonePrefab, transform.position, transform.rotation, transform);
                var pathFollower = newObject.GetComponent<PathFollower>();
                pathFollower.path = path;
                pathFollower.speed = speed;
                pathFollower.deathEffectPrefab = smokeBombPrefab;
                EffectManager.SimpleMuzzleFlash(smokeBombPrefab, gameObject, "SmokeBomb", true);

                NetworkServer.Spawn(newObject);
                lastStoneTimer = 0f;
            }
        }

    }
}
