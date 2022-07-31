using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarouselGObject : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private TextMeshProUGUI _text;

    private ICarouselElement _carouselElement;

    public string? Name 
    { 
        get
        {
            if (_carouselElement == null) return null;
            return _carouselElement.Name;
        }
    }

    public async void SetTexture() 
    {
        _rawImage.texture = await _carouselElement.GetTexture();
    }

    public void SetText(string text) 
    {
        if(_text != null)
            _text.text = text;
    }

    public Texture2D Texture { get => (Texture2D)_rawImage.texture; }

    public void OnBecomeVisible()
    { gameObject.SetActive(true); }
    public void OnBecomeInvisible()
    { gameObject.SetActive(false); }

    public void LinkCarouselElement(CarouselElement element) 
    {
        this._carouselElement = element;
        SetText(element.Name);
    }
}
