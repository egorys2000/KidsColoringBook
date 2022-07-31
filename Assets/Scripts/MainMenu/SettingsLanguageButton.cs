using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsLanguageButton : MonoBehaviour
{
    private bool _selected = false;
    public bool Selected 
    {
        get => _selected;
    }

    [SerializeField]
    private GameObject _backgroundOnSelected;

    private Button _button;

    void Awake() 
    {
        _button = GetComponent<Button>();
    }

    public void ChoseLanguage(bool choose) 
    {
        if (_selected == choose) return;

        _selected = choose;
        _backgroundOnSelected.SetActive(choose);
        _button.interactable = !choose;

        if(SettingsUIManager.Get.ChosenLanguage != this)
            SettingsUIManager.Get.ChosenLanguage.ChoseLanguage(false);
        SettingsUIManager.Get.ChosenLanguage = this;
    }
}
