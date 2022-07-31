using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SwitchColorButton : MonoBehaviour
{
    [SerializeField] private Color _color;

    public void SwitchDrawerColor() 
    {
        DrawLinesMouse.Get.ChosenColor = _color;
        DrawLinesMouse.Get.CanErase = false;
    }
}