using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that detects rotating two-finger touch input from user and rotates
/// the scene's ARSessionOrigin object (therefore rotating the scene) as the 
/// touch input rotates.
/// </summary>
public class ARRotateGesture : MonoBehaviour
{
    /// <summary>
    /// Minimum angle offset required from one frame to another for the gesture
    /// to not be ignored.
    /// </summary>
    const float ROT_ANGLE_MIN = 0;

    /// <summary>
    /// True when user is gesturing to spin.
    /// </summary>
    bool rotating;

    /// <summary>
    /// Stored value of the initial relationship between two touches when the
    /// user starts to gesture to spin.
    /// </summary>
    Vector2 startVector;

    /// <summary>
    /// Reference to the object with the scene's ARSessionOrigin component.
    /// </summary>
    [SerializeField] Transform arSessionOrigin = default;

    /// <summary>
    /// Reference to the ARSessionOrigin object's rotation as Euler angles.
    /// </summary>
    Vector3 currentRot;

    /// <summary>
    /// Set reference to scene's ARSessionOrigin object, and its starting 
    /// rotation. Rotating this object rotates the scene.
    /// </summary>
    /// <param name="_arSessionOrigin">
    /// The scene's AR Session Origin object.
    /// </param>
    public void SetARSessionOrigin(Transform _arSessionOrigin)
    {
        arSessionOrigin = _arSessionOrigin;
        currentRot = arSessionOrigin.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // Only try to rotate if exactly two touches.
        if(Input.touchCount == 2)
        {
            SpinToRotate();
            return;
        }
        rotating = false;
    }

    /// <summary>
    /// Set the ARSessionOrigin object's y rotation based on how much the
    /// user's two finger touch has rotated since it began.
    /// </summary>
    void SpinToRotate()
    {
        // If this is the first frame when the user is rotating, remember the
        // object's initial rotation and the initial relationship between the
        // touches' positions for future frames.
        if (!rotating)
        {
            startVector = 
                Input.GetTouch(1).position - Input.GetTouch(0).position;
            currentRot = arSessionOrigin.transform.rotation.eulerAngles;
            rotating = startVector.sqrMagnitude > 1;
        }
        // Figure out the difference between the user's initial and current
        // touch positions and set the ARSessionOrigin's rotation accordingly.
        else
        {
            var currVector = 
                Input.GetTouch(1).position - Input.GetTouch(0).position;
            var angleOffset = Vector2.Angle(startVector, currVector);
            var cross = Vector3.Cross(startVector, currVector);
            if (angleOffset > ROT_ANGLE_MIN)
            {
                // If z is positive add angle to rotate counterclockwise
                if (cross.z > 0)
                {
                    arSessionOrigin.rotation = 
                        Quaternion.Euler(0, currentRot.y + angleOffset, 0);
                }
                // If z is negative subtract angle to rotate clockwise
                else if(cross.z < 0)
                {
                    arSessionOrigin.rotation = 
                        Quaternion.Euler(0, currentRot.y - angleOffset, 0);
                }
            }
        }
    }
}
