using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SM64BBF
{
    public class ClonedBillboard : MonoBehaviour
    {
        private static List<Transform> instanceTransformsList;

        static ClonedBillboard()
        {
            instanceTransformsList = new List<Transform>();
            SceneCamera.onSceneCameraPreCull += OnSceneCameraPreCull;
        }

        private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
        {
            Quaternion rotation = sceneCamera.transform.rotation;
            for (int i = 0; i < instanceTransformsList.Count; i++)
            {
                // fine for now, figure out snapping later, maybe ask for help
                float y = rotation.w < 0 ? -rotation.y : rotation.y;
                instanceTransformsList[i].rotation = new Quaternion(instanceTransformsList[i].rotation.x, y, instanceTransformsList[i].rotation.z, instanceTransformsList[i].rotation.w);
            }
        }

        private void OnEnable()
        {
            instanceTransformsList.Add(base.transform);
        }

        private void OnDisable()
        {
            instanceTransformsList.Remove(base.transform);
        }
    }
}
