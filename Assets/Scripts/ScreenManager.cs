using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

/// <summary>
/// Is responsible for pop-up and scenes transmits within ChooseLevel Scene
/// </summary>
[RequireComponent(typeof(SceneTransmit))]
[RequireComponent(typeof(PopupTransmit))]
public class ScreenManager : MonoBehaviour
{
    [SerializeField]
    public ScreenEnum CurrentScreen = ScreenEnum.ChooseCategory;

    private static ScreenManager _instance;
    public static ScreenManager Get 
    {
        get => _instance;
    }

    void Awake() 
    {
        if (_instance != null)
        { 
            DestroyImmediate(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }

        SceneTransmit = GetComponent<SceneTransmit>();
        PopupTransmit = GetComponent<PopupTransmit>();
    }

    private SceneTransmit SceneTransmit;
    private PopupTransmit PopupTransmit;

    [SerializeField]
    private string _chooseLevelScene, _gameScene, _mainMenuScene;

    [SerializeField] private GameObject _categoriesScreen, _levelsScreen;
    private CarouselManager _carouselManager;
    public void SetChoseLevelReferences(GameObject categoriesScreen, GameObject levelsScreen, CarouselManager carouselManager) 
    {
        _categoriesScreen = categoriesScreen;
        _levelsScreen = levelsScreen;
        _carouselManager = carouselManager;
    }

    /// <summary>
    /// Chooses to which screen transmit automaticly
    /// </summary>
    /// <param name="deeper">if deeper = true, this means the context gets deeper (e.g. from level-category to level)</param>
    public void OpenScreenByContext(bool deeper) 
    {
        switch (CurrentScreen) 
        {
            case ScreenEnum.ChooseCategory:
                if (deeper)
                    OpenCategory(true);
                break;
                
            case ScreenEnum.ChooseLevel:
                if (deeper)
                    OpenLevel(true);
                else
                    OpenCategory(false);
                break;

            case ScreenEnum.Game:
                if (!deeper)
                    OpenLevel(false);
                break;
        }
    }

    public GameObject CategoriesScreen 
    {
        get 
        {
            if (_categoriesScreen != null) return _categoriesScreen;
            else return GameObject.FindWithTag("CategoryContent");
        }
    }
    public GameObject LevelsScreen
    {
        get
        {
            if (_levelsScreen != null) return _levelsScreen;
            else return GameObject.FindWithTag("LevelContent");
        }
    }

    /// <summary>
    /// Performs transmit from LevelCategories [Screen] to Levels [Screen]
    /// </summary>
    /// <param name="open">
    /// true if LevelCategories -> Levels
    /// false if Levels -> LevelCategories
    /// </param>
    private void OpenCategory(bool open) 
    {
        //Debug.Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        CategoriesScreen.SetActive(!open);
        LevelsScreen.SetActive(open);

        if(open) 
            CurrentScreen = ScreenEnum.ChooseLevel;
        else
            CurrentScreen = ScreenEnum.ChooseCategory;

        if (open) _carouselManager.RebuildCarousel();
    }

    /// <summary>
    /// Performs transmit from Levels [Screen] to Game [Screen]
    /// </summary>
    /// <param name="open">
    /// true if Levels -> Game
    /// false if Game -> Levels
    /// </param>
    public async Task OpenLevel(bool open)
    {
        if (open)
        {
            SceneTransmit.SceneToLoad = _gameScene;
            CurrentScreen = ScreenEnum.Game;
        }
        else 
        {
            SceneTransmit.SceneToLoad = _chooseLevelScene;
            CurrentScreen = ScreenEnum.ChooseLevel;
        }

        await SceneTransmit.SwitchSceneAsync();

        if (open)
            LevelsScreen.SetActive(false);
        else
            CategoriesScreen.SetActive(false);
    }

    public async Task ContextBackButton() 
    {
        switch (CurrentScreen)
        {
            case ScreenEnum.Game:
                CutImage.SaveFragment();
                await OpenLevel(false);
                break;
            case ScreenEnum.ChooseLevel:
                OpenCategory(false);
                break;
            case ScreenEnum.ChooseCategory:
                SceneTransmit.SceneToLoad = _mainMenuScene;
                SceneTransmit.SwitchScene();
                break;
        }
    }
}

public enum ScreenEnum 
{
    ChooseCategory = 0,
    ChooseLevel = 1,
    Game = 2
}