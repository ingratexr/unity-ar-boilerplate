using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles getting player's input by shooting out a
/// raycast from the center of the screen and getting the point where it
/// intersects the object with this component on it.
/// </summary>
public class CamRaycastInput : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Reference to main camera.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Reference to the center of the screen as vector2.
    /// </summary>
    Vector2 screenCenter;

    /// <summary>
    /// The X axis value, in local space, of the point where the camera's
    /// raycast hits this object.
    /// </summary>
    public float localXInput { get; private set; }

    /// <summary>
    /// Saved reference to the screen's orientation.
    /// </summary>
    ScreenOrientation currentOrientation;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize variables.
    /// </summary>
    private void Start()
    {
        SetScreenCenter();
        if (cam == null)
        {
            Debug.LogError("Camera was null");
            SetCamera(Camera.main);
        }
    }

    /// <summary>
    /// Save reference to camera to use for input.
    /// </summary>
    /// <param name="_cam">
    /// Camera to use for input.
    /// </param>
    private void SetCamera(Camera _cam)
    {
        cam = _cam;
    }

    /// <summary>
    /// Store a reference to the center of the screen, and a reference to the
    /// screen's orientation - if the orientation changes, the center point
    /// needs to be recalculated.
    /// </summary>
    private void SetScreenCenter()
    {
        currentOrientation = Screen.orientation;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    #endregion

    #region Methods

    // Update is called once per frame
    void Update()
    {
        // Reset screen center whenever device rotates
        if (currentOrientation != Screen.orientation)
        {
            SetScreenCenter();
        }

        ShootRaycast();
    }

    /// <summary>
    /// Shoot a raycast out from the center of the screen and get the X axis
    /// value, in local space, of the point where that raycast intersects
    /// the object with this component on it.
    /// </summary>
    private void ShootRaycast()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out hit)
            && hit.transform.gameObject == gameObject)
        {
            // Translate hit.point from world space to local space, adjusting
            // for scale, to get local space x value
            localXInput = transform.InverseTransformPoint(hit.point).x 
                * transform.localScale.x;
        }
    }

    #endregion
}
