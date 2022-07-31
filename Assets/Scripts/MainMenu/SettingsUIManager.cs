using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUIManager : MonoBehaviour
{
    private static SettingsUIManager _instance;
    public static SettingsUIManager Get 
    {
        get => _instance;
    }

    void Start() 
    {
        _instance = this;
        ChosenLanguage.ChoseLanguage(true);
    }

    [SerializeField]
    public SettingsLanguageButton ChosenLanguage;

    public void ToggleMusic(bool toggleOn) 
    {

    }
}
