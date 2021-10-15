using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Copy another camera's projection matrix and use it for this camera.
/// (Must be placed on an object with a camera component.)
/// Attach this script to any cameras that are children of an AR Camera and
/// need to have the exact same view of a scene.
/// </summary>
public class CopyCamera : MonoBehaviour
{
    /// <summary>
    /// Camera to copy.
    /// </summary>
    [SerializeField] private Camera sourceCamera = default;

    /// <summary>
    /// This camera.
    /// </summary>
    private Camera thisCamera;

    /// <summary>
    /// Cache reference to this camera.
    /// </summary>
    private void OnEnable()
    {
        thisCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// Copy the source camera's projection matrix.
    /// </summary>
    private void OnPreRender()
    {
        if (thisCamera == null || sourceCamera == null)
        {
            return;
        }
        thisCamera.projectionMatrix = sourceCamera.projectionMatrix;

    }
}
