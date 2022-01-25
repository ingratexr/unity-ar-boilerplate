using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Class that looks for surfaces to place objects in AR by raycasting out from
/// camera. Where surfaces are found, prompts user with indicator that shows where
/// the object will be placed. Places object. Should be placed on the same
/// GameObject as ARPlacementUI component.
/// </summary>
public class ARPlacementMgr : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Instance of the object to place.
    /// </summary>
    private GameObject objToPlace;

    /// <summary>
    /// Instance of the placement indicator object.
    /// </summary>
    private GameObject placementIndicator;

    /// <summary>
    /// Main AR camera (must be tagged MainCamera; is set in Start()).
    /// </summary>
    [SerializeField] private Camera ARCam = default;

    /// <summary>
    /// AR Session's raycast manager.
    /// </summary>
    private ARRaycastManager raycastMgr;

    /// <summary>
    /// AR Session's ARSessionOrigin object, used for placing the content.
    /// </summary>
    private ARSessionOrigin arSessionOrigin;

    /// <summary>
    /// The height on screen (as a percentage of the way up from the bottom) of
    /// the point from which to send out a ray to see if it hit a plane. 
    /// 0.5 (center of screen) by default but for some objects (especially tall 
    /// ones) it makes sense for it to be lower.
    /// </summary>
    private float screenPointHeight = 0.5f;

    /// <summary>
    /// The point on the screen from which the raycast is sent out looking for 
    /// planes (center of the screen by default but height can be set using 
    /// screenPointHeight).
    /// </summary>
    private Vector3 screenPoint;

    /// <summary>
    /// The pose of the point where the raycast hits a plane. Rotates with the 
    /// camera.
    /// </summary>
    private Pose placementPose;

    /// <summary>
    /// True if the raycast is hitting a plane; false otherwise.
    /// </summary>
    private bool placementPoseIsValid = false;

    /// <summary>
    /// True if we're looking for a place to put the AR content (either looking
    /// for a spot to place it in the first place, or moving it and so looking
    /// for a new spot).
    /// </summary>
    private bool doCheckForPlacing;

    /// <summary>
    /// Cached value of screen orientation when screenpoint was last set.
    /// </summary>
    private ScreenOrientation currentOrientation;

    #endregion

    #region Events and Delegates

    /// <summary>
    /// Delegate for event for when this script places an object.
    /// </summary>
    /// <param name="obj">Object this script placed.</param>
    public delegate void ObjectPlacedEventHandler(GameObject obj);

    /// <summary>
    /// Event called when this script places its object.
    /// </summary>
    public event ObjectPlacedEventHandler ObjectPlaced;

    /// <summary>
    /// Delegate for event for when this script starts moving an object.
    /// </summary>
    /// <param name="obj">Object this script is moving.</param>
    public delegate void ObjectMovingEventHandler(GameObject obj);

    /// <summary>
    /// Event called when this script starts moving its object.
    /// </summary>
    public event ObjectMovingEventHandler ObjectMoving;

    /// <summary>
    /// Delegate for event for when this script starts looking for surfaces.
    /// </summary>
    public delegate void LookingForPlanesEventHandler();

    /// <summary>
    /// Event fired when this script starts looking for surfaces. 
    /// </summary>
    public LookingForPlanesEventHandler LookingForPlanes;

    /// <summary>
    /// Delegate for event for when this script finds a surface.
    /// </summary>
    public delegate void FoundPlaneEventHandler();

    /// <summary>
    /// Event fired when this script finds a surface.
    /// </summary>
    public FoundPlaneEventHandler FoundPlane;

    #endregion

    #region Initialization

    // Initialize variables and references; get ready to face the day
    private void Awake()
    {
        if (ARCam == null)
        {
            Debug.LogError("mainCam null. Setting to Camera.main");
            ARCam = Camera.main;
        }

        // Remember the current screen orientation; set center point
        currentOrientation = Screen.orientation;
        SetScreenPoint();

        // Expects both these components to be on the AR Camera's parent object
        raycastMgr = ARCam.transform.parent.GetComponent<ARRaycastManager>();
        arSessionOrigin = 
            ARCam.transform.parent.GetComponent<ARSessionOrigin>();

        // Check for/warn about stupid
        if(raycastMgr == null || arSessionOrigin == null)
        {
            Debug.LogError("Fool! AR Placement Manager can't find Raycast " +
                "Manager and/or AR Session Origin!");
        }
    }

    /// <summary>
    /// Initialize variables and do sanity check to make sure everything is
    /// hooked up as it should be.
    /// </summary>
    private void OnEnable()
    {
        // Start by looking for planes
        OnLookingForPlanes();

        // Detect and alert to shenanigans
        if(ARCam != null && raycastMgr != null)
        {
            doCheckForPlacing = true;
        }
        else
        {
            Debug.LogError("Fool! Either mainCam or raycastMgr is null!");
            doCheckForPlacing = false;
        }
    }

    /// <summary>
    /// Sets the object to place and the object to use as a placement 
    /// indicator. Gets called when this script's object is enabled.
    /// </summary>
    /// <param name="_objToPlace">
    /// Object to place.
    /// </param>
    /// <param name="_placementIndicator">
    /// Object that shows the user where the object will go when they place it.
    /// </param>
    public void InitPlacementObjAndIndicator(
        GameObject _objToPlace,
        GameObject _placementIndicator,
        float _screenPointHeight = 0.5f)
    {
        // Make sure the GameObjects passed to this method exist in the scene
        // (i.e. aren't uninstantiated prefabs)
        if (_objToPlace.scene.rootCount == 0
            || _placementIndicator.scene.rootCount == 0)
        {
            Debug.LogError("Fool! You passed the AR Placer GameObjects " +
                "that have not been instantiated!");
            return;
        }

        // Start off with both object to place and placement indicator inactive
        objToPlace = _objToPlace;
        placementIndicator = _placementIndicator;
        objToPlace.SetActive(false);
        placementIndicator.SetActive(false);
        SetScreenPointHeight(_screenPointHeight);
    }

    /// <summary>
    /// Set the screenPointHeight.
    /// </summary>
    /// <param name="_height">
    /// New screenPointHeight.
    /// param>
    private void SetScreenPointHeight(float _height)
    {
        if (_height < 0 || _height > 1)
        {
            Debug.LogError("Fool! ScreenPointHeight is nonsense! " +
                "Setting to 0.5 by default.");
            screenPointHeight = 0.5f;
            return;
        }
        screenPointHeight = _height;
    }

    /// <summary>
    /// Set the screen point from which to raycast.
    /// </summary>
    private void SetScreenPoint()
    {
        // Screenpoint for ray cast is in the middle of the screen horizontally
        // and screenpointheight from the bottom
        screenPoint = ARCam.ViewportToScreenPoint(
            new Vector3(0.5f, screenPointHeight));
    }

    #endregion

    #region Update Methods (every frame)

    /// <summary>
    /// Update placement pose and indicator if necessary.
    /// </summary>
    void Update()
    {
        if (doCheckForPlacing)
        {
            // If phone has rotated, reset screenpoint
            if (Screen.orientation != currentOrientation)
            {
                currentOrientation = Screen.orientation;
                SetScreenPoint();
            }
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
    }

    /// <summary>
    /// Send out a raycast. If it hits a plane, set the placementPose equal to 
    /// that point, rotating it along with the camera.
    /// </summary>
    private void UpdatePlacementPose()
    {
        var hits = new List<ARRaycastHit>();
        raycastMgr.Raycast(screenPoint, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    /// <summary>
    /// Put the placementIndicator at the placementPose's position and rotation.
    /// </summary>
    private void UpdatePlacementIndicator()
    {
        // If there is no placementIndicator, abort mission
        if(placementIndicator == null)
        {
            Debug.LogError("No placement indicator instance instantiated!");
            return;
        }
        // If placementPose is legit, put the indicator there
        if (placementPoseIsValid)
        {
            // If the placementIndicatorInstance is not active, the plane is
            // newly detected this frame: make the indicator instance active
            // and fire the FoundPlane event.
            if (!placementIndicator.activeSelf)
            {
                placementIndicator.SetActive(true);

                // Start placement indicator facing camera
                placementIndicator.transform.rotation = 
                    arSessionOrigin.transform.rotation;
                OnFoundPlane();
            }

            // Use ARSessionOrigin.MakeContentAppearAt to make it look like
            // placement indicator is at the placement pose
            arSessionOrigin.MakeContentAppearAt(
                placementIndicator.transform, 
                placementPose.position);
        }

        // If the placement pose is not valid but the indicator instance is
        // active, this is the first frame when the device can't see a plane:
        // turn off the indicator and fire the LookingForPlanes event.
        else if (placementIndicator.activeSelf)
        {
            placementIndicator.SetActive(false);
            OnLookingForPlanes();
        }
    }

    #endregion

    #region Placing / Moving / Events

    /// <summary>
    /// Place the placementObjInstance at the pose's location (assumes pose's
    /// location is valid). If placementObjInstance doesn't exist, instantiates
    /// it. If it does exist, sets it to active. Turn off the placement 
    /// indicator.
    /// </summary>
    public void PlaceObject()
    {
        if (placementPoseIsValid)
        {
            // Activate the placementObj and put it where the indicator is.
            // Turn off the indicator.
            // Place it in the correct spot before enabling it so that any
            // OnEnable functions that object is calling are being called when
            // the object is in its new position.
            objToPlace.transform.position = 
                placementPose.position;
            objToPlace.transform.rotation =
                placementIndicator.transform.rotation;
            objToPlace.SetActive(true);
            placementIndicator.SetActive(false);

            // Object has been placed; stop checking for placement.
            doCheckForPlacing = false;
            
            // Fire the event.
            ObjectPlaced?.Invoke(objToPlace);
        }

#if UNITY_EDITOR
        // If in the editor, just place the thing at the world origin.
        objToPlace.transform.position = Vector3.zero;
        objToPlace.transform.rotation = Quaternion.identity;
        objToPlace.SetActive(true);
        placementIndicator.SetActive(false);
        doCheckForPlacing = false;
        ObjectPlaced?.Invoke(objToPlace);
#endif

    }

    /// <summary>
    /// Place the object somewhere else.
    /// </summary>
    public void MoveObject()
    {
        if (objToPlace != null)
        {
            objToPlace.SetActive(false);
        }
        doCheckForPlacing = true;
        OnObjectMoving();
    }

    /// <summary>
    /// Fire LookingForPlanes event.
    /// </summary>
    public void OnLookingForPlanes()
    {
        LookingForPlanes?.Invoke();
    }

    /// <summary>
    /// Fire ObjectMoving event.
    /// </summary>
    public void OnObjectMoving()
    {
        ObjectMoving?.Invoke(objToPlace);
    }

    /// <summary>
    /// Fire FoundPlane event.
    /// </summary>
    public void OnFoundPlane()
    {
        FoundPlane?.Invoke();
    }

    #endregion
}
