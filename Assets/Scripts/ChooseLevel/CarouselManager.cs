using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CarouselManager : MonoBehaviour
{
    private List<CarouselGObject> _elementsGO = new List<CarouselGObject>();

    public Texture2D GetElementTexture(int elementNumber) 
    {
        return _elementsGO[elementNumber].Texture;
    }

    [SerializeField]
    private GameObject _leftArrow, _rightArrow;

    [SerializeField]
    private GameObject _carouselElementPrefab;
    private Rect _prefabRect;
    void Awake()
    {
        _prefabRect = _carouselElementPrefab.GetComponent<RectTransform>().rect;
        _lastElementPos = _initPos;

        _spacing = GetDistanceBetweenArrows() * 0.3f;
    }

    private float GetDistanceBetweenArrows()
    {
        Vector3[] vLeft = new Vector3[4];
        Vector3[] vRight = new Vector3[4];

        _leftArrow.GetComponent<RectTransform>().GetWorldCorners(vLeft);
        _rightArrow.GetComponent<RectTransform>().GetWorldCorners(vRight);

        return vRight[0].x - vLeft[0].x;
    }

    private Vector2 _initPos
    {
        get => new Vector2(0, 0) / 2;
    }

    private float _spacing;
    private Vector2 _lastElementPos;
    private Vector2 NextElementPos()
    {
        return _lastElementPos + new Vector2(_spacing + _prefabRect.width, 0);
    }

    public void RebuildCarousel() 
    {
        foreach (var element in _elementsGO)
            Destroy(element.gameObject);
        
        _elementsGO = new List<CarouselGObject>();
        _elementNumber = 0;
        _lastElementPos = Vector2.zero;

        Debug.Log(ScreenManager.Get.CurrentScreen);

        SetupCarousel();
    }

    public void GenerateCarouselElement()
    {
        Vector2 nextElementPos = NextElementPos();

        GameObject bornGO = (GameObject)Instantiate(_carouselElementPrefab, gameObject.transform, instantiateInWorldSpace: false);
        bornGO.transform.localPosition = _lastElementPos;
        bornGO.transform.SetAsFirstSibling();
        _lastElementPos = nextElementPos;

        CarouselElement carouselElement = null;
        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseLevel)
            carouselElement = new CarouselElement
                (Content.LevelShortName((int)Content.ActiveCategory, LastElementIdx), Content.LevelIconPath((int)Content.ActiveCategory, LastElementIdx));

        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseCategory)
            carouselElement = new CarouselElement
                  (Content.CategoryName(LastElementIdx), Content.CategoryIconPath(LastElementIdx));

        bornGO.GetComponent<CarouselGObject>().LinkCarouselElement(carouselElement);
        bornGO.GetComponent<CarouselGObject>().SetTexture();

        bornGO.GetComponent<Button>().onClick.AddListener(() => 
        {
            if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseLevel)
            { 
                Content.ActiveLevel = _elementNumber;
                Content.OpenedLevelImage = GetElementTexture(_elementNumber);
            }
            if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseCategory)
                Content.ActiveCategory = _elementNumber;

            ScreenManager.Get.OpenScreenByContext(deeper: true);
        });

        _elementsGO.Add(bornGO.GetComponent<CarouselGObject>());
    }

    [SerializeField]
    private float _swipeDuration;

    // Index number of and element located at the very center between arrows
    [SerializeField] private int _elementNumber;
    public int LastElementIdx 
    {
        get => _elementsGO.Count;
    }
    private int _maxElements = 10;

    public void SwipeCarouselRight()
    {
        if (_elementNumber - 2 >= 0)
            _elementsGO[_elementNumber - 2].OnBecomeVisible();
        if (_elementNumber + 2 < _maxElements)
            _elementsGO[_elementNumber + 2].OnBecomeInvisible();

        Vector2 neighbouringElementsPositionDifference = -new Vector2(_spacing + _prefabRect.width, 0);
        SwipeCarousel(neighbouringElementsPositionDifference);
    }

    public void SwipeCarouselLeft()
    {
        if (_elementNumber - 2 >= 0)
            _elementsGO[_elementNumber - 2].OnBecomeInvisible();
        if (_elementNumber + 2 < _maxElements)
            _elementsGO[_elementNumber + 2].OnBecomeVisible();

        Vector2 neighbouringElementsPositionDifference = new Vector2(_spacing + _prefabRect.width, 0);
        SwipeCarousel(neighbouringElementsPositionDifference);
    }

    private void SwipeCarousel(Vector2 SwipeTo)
    {
        _leftArrow.GetComponent<Button>().interactable = false;
        _leftArrow.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _rightArrow.GetComponent<Button>().interactable = false;
        _rightArrow.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        _elementNumber += (int)Mathf.Sign(SwipeTo.x);

        foreach (var element in _elementsGO)
            element.transform.DOLocalMove(element.transform.localPosition - (Vector3)SwipeTo, _swipeDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if (_elementNumber > 0)
                    {
                        _leftArrow.GetComponent<Button>().interactable = true;
                        _leftArrow.GetComponent<Image>().color = Color.white;
                    }
                    if (_elementNumber < _maxElements - 1)
                    {
                        _rightArrow.GetComponent<Button>().interactable = true;
                        _rightArrow.GetComponent<Image>().color = Color.white;
                    }

                });

        GenerateNewElements();
    }

    /// <summary>
    /// Generates as many new elemets as needed
    /// </summary>
    private void GenerateNewElements() 
    {
        if(_elementsGO.Count > 0)
                _lastElementPos = (Vector2)_elementsGO[_elementsGO.Count - 1].transform.localPosition;

        while (_elementNumber + 3 > _elementsGO.Count && _elementsGO.Count < _maxElements)
        {
            GenerateCarouselElement();
        }
    }

    private bool IsVisible(CarouselGObject element) 
    {
        bool isVisible = element.transform.localPosition.x > -Screen.width && element.transform.localPosition.x < +Screen.width;

        return isVisible;
    }

    void Start() { SetupCarousel(); }

    public void SetupCarousel() 
    {
        Debug.Log(Content.CategoriesAmount);
        
        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseCategory)
            _maxElements = Content.CategoriesAmount;
        if (ScreenManager.Get.CurrentScreen == ScreenEnum.ChooseLevel)
            _maxElements = Content.LevelsAmount;

        GenerateNewElements();
    }
}
