using UnityEngine;
using UnityEditor;

public class DrawLinesBetweenChildren : MonoBehaviour
{

    void OnDrawGizmos()
    {
        Transform lastChild = null;
        foreach (Transform child in transform)
        {
            if(transform == child)
            {
                continue;
            }
            if (lastChild != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(lastChild.position, child.position);
            }

            lastChild = child;
        } 
    }
}
