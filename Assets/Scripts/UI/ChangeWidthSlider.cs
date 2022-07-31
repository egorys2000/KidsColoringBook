using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ChangeWidthSlider : MonoBehaviour
{
    public void ChangeWidth() 
    {
        DrawLinesMouse.Get.ChangeLineWidth(gameObject.GetComponent<Slider>().value);
    }
}
