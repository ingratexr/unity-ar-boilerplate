using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles the Force Update menu.
/// </summary>
public class ForceUpdate : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Text explaining that/why user needs to update to newer version
    /// </summary>
    [SerializeField] Text reason = default;

    /// <summary>
    /// Button that allows player to close this UI and skip the update
    /// </summary>
    [SerializeField] GameObject skipButton = default;

    /// <summary>
    /// Url to point user to for update. Depends on platform.
    /// </summary>
    private string updateUrl;

    #endregion

    #region Methods

    /// <summary>
    /// Set message to user about why they have to update.
    /// </summary>
    /// <param name="_reason">
    /// The reason they have to update.
    /// </param>
    public void SetReason(string _reason)
    {
        // Check for shenanigans
        if (_reason == null || _reason == "")
        {
            Debug.LogError("ForceUpdate received a nonsense reason.");
            _reason = "This version of the app is out of date. " +
                "Click to update.";
        }

        // Set reason
        reason.text = _reason;
    }

    /// <summary>
    /// Set the Url to point to the user to. Depends on platform, but it's
    /// just the Play Store / App Store address of the app.
    /// </summary>
    /// <param name="_url">
    /// Url to use.
    /// </param>
    public void SetUrl(string _url)
    {
        // This should be the case
        if (!(_url == null || _url == ""))
        {
            updateUrl = _url;
            return;
        }

        // ...but if it's not, something went wrong. Try this:
        Debug.LogError("ForceUpdate's update link is nonsense. Fix pls.");

        // In the absence of a real url (and until I know it and can hard code 
        // it here) just point to Google Play / App Store depending on platform
#if UNITY_ANDROID
        updateUrl = "https://play.google.com";
#elif UNITY_IOS
        updateUrl = "https://appstore.com";
#endif
    }

    /// <summary>
    /// Sets the skip update button to active if true, inactive if false.
    /// </summary>
    /// <param name="_allowSkip">
    /// Whether skip update button should be active or not.
    /// </param>
    public void SetSkipUpdate(bool _allowSkip)
    {
        skipButton.SetActive(_allowSkip);
    }

    /// <summary>
    /// Open the url that will update the app. 
    /// </summary>
    public void OpenUpdateUrl()
    {
        Application.OpenURL(updateUrl);
    }

    #endregion
}
