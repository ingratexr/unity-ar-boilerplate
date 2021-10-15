﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainParent : MonoBehaviour
{
    #region New Mobile AR Project Checklist

    /*  
     *  NEW PROJECT CHECKLIST:
     *      --Change platform to Android in Build Settings
     *      --Install packages:
     *          --ARFoundation (4.0.8 works) - this will also install the AR Subsystems package
     *          --ARCore XR Plugin (4.0.8 works)
     *          --ARKit XR Plugin (4.0.8 works)
     *          --DO NOT install ARKit FaceTracking - App Store will reject apps with this package unless they use the front camera
     *          --Advertisement
     *      --Project Settings -> XR Plug in Management -> check ARCore for Android and ARKit for iOS
     *      --Project Settings -> Quality -> 
     *              Shadow Resolution: High Resolution
     *              Shadow Distance: 8
     *      --Player Settings:
     *          --Company/Project name (com.ingrate.appTitle)
     *          --Add default Ingrate icon
     *          --Splash image - add Ingrate vertical logo, change BG color to white, other setting to dark on light
     *          --Android scripting backend: IL2CPP, not Mono
     *          --Android target architectures: ARMv7 and ARM64
     *          --Android set min/target API level (29 works as of January 2021)
     *          --iOS signing team ID - add Apple ID email address
     *          --iOS - check "requires ARKit support", change minimum iOS version
     *          --iOS architecture: ARM64, not universal
     *          --Add camera, microphone, location usage descriptions
     *      --In the scene
     *          --Remember to add an Event System
     *      
     */

    #endregion

    #region Magic Editor Constants

    /// <summary>
    /// Arbitrary position for camera in Editor. Assumes AR scene is at origin.
    /// Needs to be adjusted to size of AR scene project by project.
    /// </summary>
    Vector3 EDITOR_CAM_POSITION = new Vector3 (0, 2.5f, -3f);

    /// <summary>
    /// Arbitrary rotation for camera in Editor. Assumes AR scene is at origin.
    /// Needs to be adjusted to specific viewing angle for AR scene project by
    /// project.
    /// </summary>
    Vector3 EDITOR_CAM_ROTATION = new Vector3(45f, 0, 0);

    /// <summary>
    /// Arbitrary background color in the Editor.
    /// </summary>
    Color EDITOR_BG_COLOR = new Color(0.5f, 0.5f, 0.5f);

    #endregion

    #region Variables

    /// <summary>
    /// Reference to the AR Camera (main camera) in the scene.
    /// </summary>
    [SerializeField] Camera ARCamera = default;

    /// <summary>
    /// Reference to the Transform of the GameObject with the ARSessionOrigin
    /// component on it.
    /// </summary>
    [SerializeField] Transform ARSessionOrigin = default;

    /// <summary>
    /// Reference to the AR Placement Manager object.
    /// </summary>
    [SerializeField] ARPlacementMgr arPlacer = default;

    /// <summary>
    /// Reference to the main UI.
    /// </summary>
    [SerializeField] MainUI mainUI = default;

    /// <summary>
    /// Reference to the object/AR scene to place in space.
    /// This script will instantiate an instance of it when the game starts.
    /// </summary>
    [SerializeField] GameObject arSceneObject = default;

    /// <summary>
    /// Reference to the placement guide object to help the user place the AR
    /// scene in space.
    /// This script will instantiate an instance of it when the game starts.
    /// </summary>
    [SerializeField] GameObject arScenePlacer = default;

    /// <summary>
    /// Instantiated instance of arSceneObject.
    /// </summary>
    GameObject arSceneObjectInstance;

    /// <summary>
    /// Instantiated instance of arScenePlacer.
    /// </summary>
    GameObject arScenePlacerInstance;

    #endregion

    #region Initialization

    /// <summary>
    /// Set MainUI active and AR Placement UI inactive. If in the editor, move
    /// the camera to the preset position/rotation.
    /// </summary>
    private void Awake()
    {
        OpenMainUI();
        SetEditorView();
    }

    /// <summary>
    /// If in the Editor, sets camera to position, rotation and background
    /// color specified by constants. Otherwise sets camera position and
    /// rotation to zero.
    /// </summary>
    private void SetEditorView()
    {
#if UNITY_EDITOR
        // If in the editor, move and rotate camera to be able to see content
        ARCamera.transform.position = EDITOR_CAM_POSITION;
        ARCamera.transform.rotation = Quaternion.Euler(EDITOR_CAM_ROTATION);
        ARCamera.backgroundColor = EDITOR_BG_COLOR;
#endif
    }

#endregion

    #region Methods

    /// <summary>
    /// Instantiate/initialize ar scene object and placer; open ar placement
    /// UI/close main menu. (ie: Start the Game.)
    /// </summary>
    public void StartGame()
    {
        // Instantiate instances of the scene object and placer.
        arSceneObjectInstance = Instantiate(arSceneObject);
        arScenePlacerInstance = Instantiate(arScenePlacer);

        // Initialize the placer object's ARPinchZoom and ARRotateGesture
        // components. This structure assumes those components will sit on the
        // placement object because it ensures that the user can only pinch-to-
        // zoom and twist-to-rotate when the placer is active - once the scene
        // is placed they'll have to open the placer again to adjust its size,
        // position, rotation.
        InitPlacer(arScenePlacerInstance);

        // Open the AR placement UI and initialize it with the instances of the
        // object and placer.
        OpenARPlacementUI();
        arPlacer.InitPlacementObjAndIndicator(arSceneObjectInstance, 
            arScenePlacerInstance);
    }

    /// <summary>
    /// Takes an instance of the AR scene placer and sets its ARPinchZoom and 
    /// ARRotateGesture components' references, if they exist, to the scene's 
    /// ARSessionOrigin object.
    /// </summary>
    /// <param name="_placerInstance">
    /// The instantiated instance of the AR scene's placer object.
    /// </param>
    private void InitPlacer(GameObject _placerInstance)
    {
        ARPinchZoom pz = _placerInstance.GetComponent<ARPinchZoom>();
        if(pz != null)
        {
            pz.InitSizeAndARSesh(ARSessionOrigin);
        }
        ARRotateGesture rg = _placerInstance.GetComponent<ARRotateGesture>();
        if (rg != null)
        {
            rg.SetARSessionOrigin(ARSessionOrigin);
        }
    }

    /// <summary>
    /// Destroy arSceneObject and placer instances, close the AR placement UI, 
    /// return to main UI (and portrait mode).
    /// </summary>
    public void OpenMainUI()
    {
        DestroyObjectAndPlacerInstances();
        ForcePortrait();
        arPlacer.gameObject.SetActive(false);
        mainUI.gameObject.SetActive(true);
        mainUI.OpenMainMenu();
    }

    /// <summary>
    /// Close the main UI, open the AR placement UI, and allow the user to 
    /// rotate to switch between landscape and portrait modes.
    /// </summary>
    public void OpenARPlacementUI()
    {
        AllowAutoRotate();
        arPlacer.gameObject.SetActive(true);
        mainUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Allow screen to switch between Portrait and Landscape modes as user
    /// rotates device.
    /// </summary>
    public void AllowAutoRotate()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    /// <summary>
    /// Force screen to stay in Portrait mode regardless of device orientation.
    /// </summary>
    public void ForcePortrait()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToLandscapeLeft = false;
    }

    /// <summary>
    /// Destroy arSceneObjectInstance and arScenePlacer instance if they exist.
    /// </summary>
    private void DestroyObjectAndPlacerInstances()
    {
        if (arSceneObjectInstance != null)
        {
            Destroy(arSceneObjectInstance);
        }
        if (arScenePlacerInstance != null)
        {
            Destroy(arScenePlacerInstance);
        }
    }

    #endregion

}