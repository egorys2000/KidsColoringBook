using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Palette : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _palettes;

    [SerializeField]
    private Image _image;

    void Start()
    {
        SetRandomPalette();
    }

    public void SetRandomPalette() 
    {
        _image.sprite = _palettes[Random.Range(0, _palettes.Count)];
    }
}
