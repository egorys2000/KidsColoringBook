using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseLevelUICorrection : MonoBehaviour
{
    [SerializeField]
    private GameObject CategoriesScreen, LevelsScreen;

    [SerializeField] private CarouselManager LevelsCarouselManager;

    void Start()
    {
        CategoriesScreen.SetActive(true);
        LevelsScreen.SetActive(true);

        ScreenManager.Get.SetChoseLevelReferences(
            categoriesScreen: CategoriesScreen,
            levelsScreen: LevelsScreen,
            carouselManager: LevelsCarouselManager
            );

        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseCategory) LevelsScreen.SetActive(false);
        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseLevel) CategoriesScreen.SetActive(false);
    }
}
