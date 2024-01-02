using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class previewLevel : MonoBehaviour
{
    public Level prevLevel;

    void OnDrawGizmosSelected()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(prevLevel.levelWidth, 0, prevLevel.levelHeight));
    }
}
