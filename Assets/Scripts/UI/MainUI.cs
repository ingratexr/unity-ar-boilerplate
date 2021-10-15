using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Array that holds the parent objects of all the menus.
    /// </summary>
    GameObject[] allMenus;

    /// <summary>
    /// Parent object of Main Menu.
    /// </summary>
    [SerializeField] GameObject mainMenuParent = default;

    /// <summary>
    /// Parent object of About Menu.
    /// </summary>
    [SerializeField] GameObject aboutMenuParent = default;

    /// <summary>
    /// Parent object of Forced Update UI.
    /// </summary>
    [SerializeField] GameObject forceUpdateParent = default;

    #endregion

    #region Initialization

    /// <summary>
    /// Fill the array of all menus with all the menus; open main menu.
    /// </summary>
    private void Awake()
    {
        allMenus = new GameObject[] {
            mainMenuParent,
            aboutMenuParent,
            forceUpdateParent,
        };

        OpenMainMenu();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Set the menu passed to it active and all other menus in allMenus array
    /// inactive.
    /// </summary>
    /// <param name="_menu">
    /// The menu to open. Must be in allMenus array.
    /// </param>
    private void OpenMenu(GameObject _menu)
    {
        for(int i = 0; i < allMenus.Length; i++)
        {
            allMenus[i].SetActive(allMenus[i] == _menu);
        }
    }

    /// <summary>
    /// Use a string to select a menu to open.
    /// </summary>
    /// <param name="_menu">
    /// The menu to open.
    /// </param>
    public void PickAMenu(string _menu)
    {
        // Set to all lowercase in case of accidental capitalization
        _menu = _menu.ToLower();

        // Open the correct menu if possible
        switch (_menu)
        {
            case "main":
                OpenMainMenu();
                break;
            case "about":
                OpenAboutMenu();
                break;
            case "forceupdate":
                Debug.LogError("Cannot open forced update menu without args!");
                OpenMainMenu();
                break;
            default:
                Debug.LogError("Menu choice is nonsense! You get main menu.");
                OpenMainMenu();
                break;
        }
    }

    /// <summary>
    /// Open the main menu and close other UI.
    /// </summary>
    public void OpenMainMenu()
    {
        OpenMenu(mainMenuParent);
    }

    /// <summary>
    /// Open the info menu and close other UI.
    /// </summary>
    public void OpenAboutMenu()
    {
        OpenMenu(aboutMenuParent);
    }

    /// <summary>
    /// Initialize force update UI with correct url and message to user. Open
    /// force update UI and close other UI.
    /// </summary>
    /// <param name="_reason"></param>
    /// <param name="_url"></param>
    public void ForceUpdate(string _reason, string _url, bool _allowSkip)
    {       
        // If force update, the main menu object needs to turn itself on
        gameObject.SetActive(true);

        // Set the variables correctly
        ForceUpdate fu = forceUpdateParent.GetComponent<ForceUpdate>();
        fu.SetReason(_reason);
        fu.SetUrl(_url);
        fu.SetSkipUpdate(_allowSkip);

        // Open dat bitch
        OpenMenu(forceUpdateParent);
    }

    #endregion

}
