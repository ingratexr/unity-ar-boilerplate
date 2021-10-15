using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;

public class AdMgr : MonoBehaviour
{
    #region Magic Constants

    /// <summary>
    /// The type of ad to play.
    /// </summary>
    private const string adType = "rewardedVideo";

    /// <summary>
    /// Unity ID for iOS ads.
    /// </summary>
    const string iOSID = "ERROR"; //  You gotta create this with Unity

    /// <summary>
    /// Unity ID for Android ads.
    /// </summary>
    const string androidID = "ERROR"; // You gotta create this with Unity

    #endregion

    #region Variables

    /// <summary>
    /// When true, player will have to place AR content again when ad has
    /// finished playing (to avoid it having moved/shifted/etc during ad).
    /// </summary>
    [SerializeField] private bool moveAROnAdFinished = true;

    /// <summary>
    /// Reference to app's AR Placer object.
    /// </summary>
    [SerializeField] private ARPlacementMgr arPlacer = default;

    #endregion

    #region Events and Delegates

    /// <summary>
    /// Event handler for AdStarted event.
    /// </summary>
    public delegate void AdStartedEventHandler();

    /// <summary>
    /// Fires when ad has started.
    /// </summary>
    public AdStartedEventHandler AdStarted;

    /// <summary>
    /// Event handler for AdFinished event.
    /// </summary>
    public delegate void AdFinishedEventHandler();

    /// <summary>
    /// Fires when ad finished.
    /// </summary>
    public AdFinishedEventHandler AdFinished;

    /// <summary>
    /// Event handler for AdFailed event.
    /// </summary>
    public delegate void AdFailedEventHandler();

    /// <summary>
    /// Fires if ad fails.
    /// </summary>
    public AdFailedEventHandler AdFailed;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize advertisements.
    /// </summary>
    private void Start()
    {
        Advertisement.Initialize(GameID(), TestMode());
    }

    /// <summary>
    /// Returns true when in Editor, false otherwise.
    /// </summary>
    private bool TestMode()
    {
#if UNITY_EDITOR
        return true;
#else 
        return false;
#endif
    }

    /// <summary>
    /// Returns iOSID when iOS and AndroidID when Android. If neither, 
    /// returns AndroidID.
    /// </summary>
    private string GameID()
    {
#if UNITY_IOS
        return iOSID;
#elif UNITY_ANDROID
        return androidID;
#else
        Debug.LogError("AdCtrl could not set gameID based on platform; " +
        "returning Android ID");
        return androidID;
#endif
    }

#endregion

    #region Methods

    /// <summary>
    /// Tries to show ad. If can't, calls method to fire AdFailed event.
    /// </summary>
    public void ShowAd()
    {
        if (Advertisement.IsReady(adType))
        {
            Advertisement.Show(adType, 
                new ShowOptions() { resultCallback = HandleAdResult });
            OnAdStarted();
            return;
        }
        OnAdFailed();
    }

    /// <summary>
    /// Callback that handles result of trying to show ad.
    /// </summary>
    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                OnAdFinished();
                break;
            case ShowResult.Failed:
                OnAdFailed();
                break;
            case ShowResult.Skipped:
                Debug.Log("skipped");
                OnAdFinished();
                break;
        }
    }

    /// <summary>
    /// Fires Ad Started event.
    /// </summary>
    private void OnAdStarted()
    {
        AdStarted?.Invoke();
    }

    /// <summary>
    /// Fires Ad finished event, and calls AR Placer logic to move AR object 
    /// if moveAROnAdFinished.
    /// </summary>
    private void OnAdFinished()
    {
        AdFinished?.Invoke();
        if (moveAROnAdFinished)
        {
            arPlacer.MoveObject();
        }
    }

    /// <summary>
    /// Fires AdFailed event.
    /// </summary>
    private void OnAdFailed()
    {
        AdFailed?.Invoke();
    }

    #endregion
}
