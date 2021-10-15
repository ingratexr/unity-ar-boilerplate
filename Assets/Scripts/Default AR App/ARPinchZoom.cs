using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that enables user to pinch to change scale of AR scene. 
/// Will change scale of the whole scene/all objects - don't use this class to 
/// pinch to zoom individual objects.
/// </summary>
 
public class ARPinchZoom : MonoBehaviour
{

    #region Magic Constants

    /// <summary>
    /// Speed at which scale changes.
    /// 0.01 is arbitrary but seems good/not frustrating.
    /// </summary>
    private const float ZOOM_SPEED = 0.01f;

    #endregion

    #region Variables

    /// <summary>
    /// Object with the scene's ARSessionOrigin script on it. ARPinchZoom class
    /// changes this object's scale to change the scale of the AR scene.
    /// </summary>
    Transform ARSessionParent;

    /// <summary>
    /// Default value for minimum scale.
    /// Larger value = smaller scale.
    /// </summary>
    float minScale = 4f;

    /// <summary>
    /// Default value for maximum scale.
    /// Smaller value = larger scale.
    /// </summary>
    float maxScale = 0.25f;

    /// <summary>
    /// Default max and min size values to use.
    /// It doesn't matter whether the max,min are input as x,y or y,x. The
    /// setter sets the larger value as the minimum and the smaller value as
    /// the maximum (larger value = smaller scale).
    /// By default this vector2 is (0,0). If it's still (0,0) when this script 
    /// tries to use it (ie it hasn't been set in the Inspector window) the 
    /// script will throw an error and use minScale and maxScale defaults 
    /// defined above.
    /// </summary>
    [SerializeField] Vector2 defaultSizeRange = default;

    #endregion

    #region Initialization and Settings

    /// <summary>
    /// Initialize this object using the default size range.
    /// </summary>
    /// <param name="_arSessionParent">
    /// GameObject that has scene's ARSessionParent component on it.
    /// </param>
    public void InitSizeAndARSesh(Transform _arSessionParent)
    {
        SetARSessionParent(_arSessionParent);
        SetSize(defaultSizeRange);
    }

    /// <summary>
    /// Initialize this object specifying a non-default size range.
    /// </summary>
    /// <param name="_arSessionParent">
    /// GameObject that has scene's ARSessionParent component on it.
    /// </param>
    /// <param name="_sizeRange">
    /// Max and min size values to use.
    /// </param>
    public void InitSizeAndARSesh(
        Transform _arSessionParent, 
        Vector2 _sizeRange)
    {
        SetARSessionParent(_arSessionParent);
        SetSize(_sizeRange);
    }

    /// <summary>
    /// Set the reference to the object with the scene's ARSessionOrigin
    /// component attached to it.
    /// </summary>
    /// <param name="_arSessionParent">
    /// Object with scene's ARSessionOrigin component attached to it.
    /// </param>
    private void SetARSessionParent(Transform _arSessionParent)
    {
        // Make sure the game object has the ARSessionOrigin script, otherwise
        // pinching/zooming/etc isn't gonna work
        if (_arSessionParent.GetComponent<UnityEngine.XR.ARFoundation
            .ARSessionOrigin>() == null)
        {
            Debug.LogError("Disaster! ARSessionParent object doesn't " +
                "have ARSessionOrigin script. Scaling isn't gonna work.");
        }
        ARSessionParent = _arSessionParent;
    }

    /// <summary>
    /// Set the minimum and maximum sizes with a vector2, where the larger
    /// value is the minimum and the smaller value is the maximum.
    /// </summary>
    /// <param name="_sizeRange">
    /// Minimum and maximum scale: larger value is minimum, smaller value is
    /// maximum.
    /// </param>
    private void SetSize(Vector2 _sizeRange)
    {
        // Refuse zeros or negative numbers as input.
        // Zeros (probably) won't break the logic, but also probably mean
        // someone forgot to pass a real value.
        // There's no telling what kind of deviated prevert would try to pass
        // an innocent method like this negative numbers, or what would happen
        // if decent people allowed it, so, no, Karen, positive only please.
        if (_sizeRange.x <= 0 || _sizeRange.y <= 0)
        {
            Debug.LogError("Got nonsense for size range! Did you forget to " +
                "set it for this object? Reverting to default min/max.");
            return;
        }
        
        // The minimum scale is the larger value and the maximum scale is the
        // smaller value. Larger scale value means scene appear smaller.
        minScale = Mathf.Max(_sizeRange.x, _sizeRange.y);
        maxScale = Mathf.Min(_sizeRange.x, _sizeRange.y);

        // Set the starting scale to the average of the min and max.
        float avg = (minScale + maxScale) / 2f;
        ARSessionParent.transform.localScale = new Vector3(avg, avg, avg);
    }

    #endregion

    #region Where the Magic Happens

    // Update is called once per frame
    void Update()
    {
        PinchToZoom();
    }

    /// <summary>
    /// If there are two fingers touching the screen, make the scale bigger and
    /// smaller as the fingers pinch closer or farther apart.
    /// </summary>
    void PinchToZoom()
    {
        // Only do any of this if there are exactly two touches
        if(Input.touchCount != 2)
        {
            return;
        }

        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Get the touches' positions in the previous frame of each touch.
        Vector2 touchZeroPrevPos = 
            touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = 
            touchOne.position - touchOne.deltaPosition;

        // Find the distance between the touches in each frame.
        float prevTouchDeltaMag = 
            (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = 
            (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // Rescale
        // The function here adds the product of deltaMagDiff and zoomSpeed 
        // because as the pinching fingers get closer the number gets bigger to
        // make the object smaller (the bigger the number, the smaller the 
        // scale of AR objects).
        // To implement this idea somewhere with the reverse, more intuitive
        // version (making both the scale number and the appearance smaller as
        // the fingers get closer), subtract the product.
        // Much confusing. Thanks Unity.
        float newScale = ARSessionParent.localScale.x
            + (deltaMagnitudeDiff * ZOOM_SPEED);
        newScale = Mathf.Clamp(newScale, maxScale, minScale);
        ARSessionParent.localScale = 
            new Vector3(newScale, newScale, newScale);
    }

    #endregion
}
