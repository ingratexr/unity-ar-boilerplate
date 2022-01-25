using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that automatically update value for how much canvas is matching width
/// vs height to reference canvas size.
/// </summary>
public class CanvasAspect : MonoBehaviour
{
    #region Magic Constants

    /// *************
    /// These values are based on what I think looks good for the aspect ratios
    /// of an iPhoneX, iPad, and iPhone 6 / Pixel XL, assuming a portrait menu 
    /// item aspect ratio of 1:2 (this is what St Nick uses)
    /// 
    /// Match width/height value gets smaller (matching width more closely) as
    /// portrait aspect ratio gets narrower/taller, and as landscape aspect
    /// ratio gets wider/fatter.
    /// *************

    /// <summary>
    /// When portrait, aspect ratio at or below which match value goes to its
    /// minimum (portraitMin; 0 by default).
    /// </summary>
    private const float PORT_MIN = 0.5f;

    /// <summary>
    /// When portrait, aspect ratio at or above which match value goes to its
    /// maximum (portraitMax; 1 by default).
    /// </summary>
    private const float PORT_MAX = 0.75f;

    /// <summary>
    /// When landscape, aspect ratio at or above which match value goes to its
    /// minimum (landscapeMin; 0.75 by default).
    /// </summary>
    private const float LAND_MAX = 1.75f;

    /// <summary>
    /// When landscape, aspect ratio at or below which match value goes to its
    /// maximum (landscapeMax; 1 by default).
    /// </summary>
    private const float LAND_MIN = 1.33f;

    #endregion

    #region Variables

    /// <summary>
    /// Reference to main camera.
    /// </summary>
    [SerializeField] Camera mainCam = default;

    /// <summary>
    /// Reference to canvas to resize.
    /// </summary>
    [SerializeField] UnityEngine.UI.CanvasScaler canvasScaler = default; //     << TO DO: Make this an array to iterate through

    /// *************
    /// These values are based on what I think looks good for the aspect ratios
    /// of an iPhoneX, iPad, and iPhone 6 / Pixel XL, assuming a portrait menu 
    /// item aspect ratio of 1:2 (this is what St Nick uses)
    /// *************

    /// <summary>
    /// Minimum width/height match value when portrait.
    /// </summary>
    //[SerializeField] 
    float portraitMin = 0;

    /// <summary>
    /// Maximum width/height match value when portrait.
    /// </summary>
    //[SerializeField] 
    float portraitMax = 1f;

    /// <summary>
    /// Minimum width/height match value when landscape.
    /// </summary>
    //[SerializeField] 
    float landscapeMin = 0.75f;

    /// <summary>
    /// Maximum width/height match value when landscape.
    /// </summary>
    //[SerializeField] 
    float landscapeMax = 1f;

    /// <summary>
    /// Cached value of screen orientation when changes were last made.
    /// </summary>
    ScreenOrientation orient;

    #endregion

    /// <summary>
    /// Whenever this object is enabled, re-check/set everything.
    /// </summary>
    private void OnEnable()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
        orient = Screen.orientation;
        SetMatchValue();
    }

    /// <summary>
    /// Check if screen orientation has changed and if so, recalculate 
    /// everything.
    /// </summary>
    private void Update()
    {
        if(orient != Screen.orientation)
        {
            orient = Screen.orientation;
            SetMatchValue();
        }
    }

    /// <summary>
    /// Figure out screen's aspect ratio and set canvas scaler accordingly.
    /// </summary>
    private void SetMatchValue()
    {
        var aspect = AspectRatio();
        //float value = aspect >= 1 
        //    ? LandscapeValue(aspect) 
        //    : PortraitValue(aspect);
        //canvasScaler.matchWidthOrHeight = value;
        canvasScaler.matchWidthOrHeight = aspect >= 1
            ? LandscapeValue(aspect)
            : PortraitValue(aspect);
    }

    /// <summary>
    /// Returns screen's aspect ratio as decimal.
    /// </summary>
    private float AspectRatio()
    {
        // Uses pixelWidth and not screen.width because screen.width doesn't 
        // work in the editor.
        return (float)mainCam.pixelWidth / (float)mainCam.pixelHeight;
    }

    /// <summary>
    /// Takes a landscape aspect ratio and returns the value the canvas scaler
    /// should use to match width vs height.
    /// </summary>
    /// <param name="_aspect">
    /// Screen's aspect ratio as decimal.
    /// </param>
    private float LandscapeValue(float _aspect)
    {
        float pctFromMinToMax = 
            (Mathf.Clamp(_aspect, LAND_MIN, LAND_MAX) - LAND_MIN) 
            / (LAND_MAX-LAND_MIN);
        // Lerp from bigger to smaller number
        return Mathf.Lerp(landscapeMax, landscapeMin, pctFromMinToMax);
    }

    /// <summary>
    /// Takes a portrait aspect ratio and returns the value the canvas scaler
    /// should use to match width vs height.
    /// </summary>
    /// <param name="_aspect">
    /// Screen's aspect ratio as decimal.
    /// </param>
    private float PortraitValue(float _aspect)
    {
        float pctFromMinToMax = 
            (Mathf.Clamp(_aspect, PORT_MIN, PORT_MAX) - PORT_MIN) 
            / (PORT_MAX - PORT_MIN);
        return Mathf.Lerp(portraitMin, portraitMax, pctFromMinToMax);
    }
}
