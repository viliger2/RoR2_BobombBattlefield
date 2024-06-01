using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SM64BBF
{
    public class ClonedBlillboard : MonoBehaviour
    {
        private static List<Transform> instanceTransformsList;

        static ClonedBlillboard()
        {
            instanceTransformsList = new List<Transform>();
            SceneCamera.onSceneCameraPreCull += OnSceneCameraPreCull;
        }

        private static void OnSceneCameraPreCull(SceneCamera sceneCamera)
        {
            Quaternion rotation = sceneCamera.transform.rotation;
            //Log.Debug($"X: {sceneCamera.transform.rotation.x}, Y: {sceneCamera.transform.rotation.y}, Z: {sceneCamera.transform.rotation.z}, W: {sceneCamera.transform.rotation.w}");
            
            for (int i = 0; i < instanceTransformsList.Count; i++)
            {
                //var newRot = Quaternion.LookRotation(sceneCamera.transform.position - instanceTransformsList[i].position);
                // var cRot = instanceTransformsList[i].rotation;
                // newRot.eulerAngles= new Vector3(cRot.eulerAngles.x, newRot.eulerAngles.y, cRot.eulerAngles.z);
                // instanceTransformsList[i].rotation = newRot;

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
