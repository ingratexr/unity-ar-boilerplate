using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles navigation for Info Menu.
/// </summary>
public class AboutMenu : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Url that points to privacy policy.
    /// </summary>
    string privacyPolicyUrl = "https://ingratexr.com/privacy";

    /// <summary>
    /// Url that points to a page with more info about Ingrate
    /// </summary>
    string ingrateInfoUrl = "https://ingratexr.com";

    /// <summary>
    /// Url that points to terms and conditions to subject user to
    /// </summary>
    string termsConditionsUrl = "https://ingratexr.com";

    #endregion

    #region Methods

    /// <summary>
    /// Set the privacy policy url
    /// </summary>
    /// <param name="_url">
    /// Privacy policy url to use
    /// </param>
    public void SetPrivacyUrl(string _url)
    {
        SetAnyUrl(ref privacyPolicyUrl, _url);
    }

    /// <summary>
    /// Set the Ingrate info url
    /// </summary>
    /// <param name="_url">
    /// Ingrate info url to use
    /// </param>
    public void SetIngrateInfoUrl(string _url)
    {
        SetAnyUrl(ref ingrateInfoUrl, _url);
    }

    /// <summary>
    /// Set the terms and conditions url
    /// </summary>
    /// <param name="_url">
    /// Terms and conditions url to use
    /// </param>
    public void SetTermsConditionsUrl(string _url)
    {
        SetAnyUrl(ref termsConditionsUrl, _url);
    }

    /// <summary>
    /// Set any of the url variables. Takes a ref to the string you want to
    /// change, checks that you're not changing it to null or an empty string,
    /// and changes it to the url you want.
    /// </summary>
    /// <param name="_var">
    /// Variable to change
    /// </param>
    /// <param name="_url">
    /// Url to change it to
    /// </param>
    private void SetAnyUrl(ref string _var, string _url)
    {
        // Check for null/empty strings. If nonsense found, leave as defaults.
        if (_url == null || _url == "")
        {
            Debug.LogError(_var + " got a nonsense url");
            return;
        }
        _var = _url;
    }

    /// <summary>
    /// Open the privacy policy url in browser
    /// </summary>
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyUrl);
    }

    /// <summary>
    /// Open the Ingrate Info url in browser
    /// </summary>
    public void OpenIngrateInfo()
    {
        Application.OpenURL(ingrateInfoUrl);
    }

    /// <summary>
    /// Open the terms and conditions url in browser
    /// </summary>
    public void OpenTermsConditions()
    {
        Application.OpenURL(termsConditionsUrl);
    }

#endregion
}
