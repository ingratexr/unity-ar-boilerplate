using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles UI related to placing an AR object using an ARPlacer
/// component. Needs to be attached to the same GameObject as the ARPlacer
/// component.
/// </summary>
public class ARPlacementUI : MonoBehaviour
{
    #region Magic Constants

    /// <summary>
    /// Distance in front of the camera to place handPhoneInstance.
    /// </summary>
    private const float HAND_PHONE_FWD_OFFSET = 2.5f;

    /// <summary>
    /// How many seconds to wait while seeking planes before showing 
    /// HavingTrouble? prompt.
    /// </summary>
    private const int SECONDS_UNTIL_HELP_PROMPT = 10;

    #endregion

    #region Variables

    /// <summary>
    /// Button to start the placement process again, if the user wants to move
    /// the object after it's been placed.
    /// </summary>
    [SerializeField] GameObject moveButton = default;

    /// <summary>
    /// Button to place the object where it is.
    /// </summary>
    [SerializeField] GameObject placeButton = default;

    /// <summary>
    /// Text that appears when seeking a horizontal surface.
    /// </summary>
    [SerializeField] GameObject seekingSurfacesText = default;

    /// <summary>
    /// Button to take player to troubleshooting menu.
    /// </summary>
    [SerializeField] GameObject havingTrouble = default;

    /// <summary>
    /// Parent object of help menu.
    /// </summary>
    [SerializeField] GameObject popupHelp = default;

    ///// <summary>
    ///// AR Camera in the scene.
    ///// </summary>
    //[SerializeField] Camera arCamera = default;

    /// <summary>
    /// AR Placer object on this GameObject.
    /// </summary>
    ARPlacementMgr arPlacer;

    /// <summary>
    /// True when counting time before showing HavingTrouble? prompt.
    /// </summary>
    bool runTimer;

    /// <summary>
    /// Reference to time in seconds when timer starts.
    /// </summary>
    int timeAtStartCounting;

    #endregion

    #region Methods

    /// <summary>
    /// Initialize variables and objects.
    /// </summary>
    private void Awake()
    {
        // Get reference to AR Placer and subscribe to events.
        arPlacer = GetComponent<ARPlacementMgr>();
        arPlacer.ObjectPlaced += ObjectPlaced;
        arPlacer.ObjectMoving += ObjectMoving;
        arPlacer.LookingForPlanes += LookingForPlanes;
        arPlacer.FoundPlane += FoundPlane;

        ClosePopupHelp();
    }

    /// <summary>
    /// If timer is running, check to see whether enough time has passed to 
    /// turn on the "HavingTrouble?" prompt.
    /// </summary>
    private void Update()
    {
        if (runTimer)
        {
            if ((int)Time.time - timeAtStartCounting 
                >= SECONDS_UNTIL_HELP_PROMPT)
            {
                OpenHavingTrouble();
                runTimer = false;
            }
        }
    }

    /// <summary>
    /// OnEnable, always start by looking for planes.
    /// </summary>
    private void OnEnable()
    {
        LookingForPlanes();
        //Screen.orientation = ScreenOrientation.AutoRotation;
    }

    ///// <summary>
    ///// OnDisable, 
    ///// </summary>
    //private void OnDisable()
    //{
    //    //Screen.orientation = ScreenOrientation.Portrait;
    //}

    /// <summary>
    /// Called when object is placed. Turns off placement UI and turns on move
    /// button.
    /// </summary>
    /// <param name="obj">
    /// The object that was placed. Reference exists to match delegate. Ignore.
    /// </param>
    private void ObjectPlaced(GameObject obj)
    {
        placeButton.SetActive(false);
        moveButton.SetActive(true);
        seekingSurfacesText.SetActive(false);
        havingTrouble.SetActive(false);
        runTimer = false;
        ClosePopupHelp();
    }

    /// <summary>
    /// Called when user chooses to move object. Starts looking for planes.
    /// </summary>
    /// <param name="obj">
    /// The object that's moving. Reference exists to match delegate. Ignore.
    /// </param>
    private void ObjectMoving(GameObject obj)
    {
        LookingForPlanes();
    }

    /// <summary>
    ///  Called when ARPlacer starts looking for planes. Turns on seeking
    ///  surfaces and handPhoneInstance and turns off irrelevant UI. Starts/
    ///  restarts timer counting towards showing HavingTrouble? prompt.
    /// </summary>
    private void LookingForPlanes()
    {
        placeButton.SetActive(false);
        moveButton.SetActive(false);
        seekingSurfacesText.SetActive(true);
        havingTrouble.SetActive(false);

        // Restart the HavingTrouble? prompt timer each time this method runs
        timeAtStartCounting = (int)Time.time;
        runTimer = true;

        // If in the editor, immediately allow user to place object
#if UNITY_EDITOR
        placeButton.SetActive(true);
#endif
    }

    /// <summary>
    /// Called when ARPlacer has found a surface where the object can go.
    /// Turns on PlaceButton and turns off irrelevant UI.
    /// </summary>
    private void FoundPlane()
    {
        placeButton.SetActive(true);
        moveButton.SetActive(false);
        seekingSurfacesText.SetActive(false);
        havingTrouble.SetActive(false);
        runTimer = false;

        ClosePopupHelp();
    }

    /// <summary>
    /// Open the HavingTrouble? prompt.
    /// </summary>
    private void OpenHavingTrouble()
    {
        havingTrouble.SetActive(true);
        seekingSurfacesText.SetActive(false);
    }

    /// <summary>
    /// Open the help menu and pause time.
    /// </summary>
    public void OpenPopupHelp()
    {
        popupHelp.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Close the help menu and unpause time.
    /// </summary>
    public void ClosePopupHelp()
    {
        popupHelp.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion
}
