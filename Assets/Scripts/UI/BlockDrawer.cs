using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockDrawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        DrawLinesMouse.Get.ClickedOnUI = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(SkipFrameThenInvokeCallback(Callback: new Action(() => 
        {
            DrawLinesMouse.Get.ClickedOnUI = false;
        })) );
    }

    private IEnumerator SkipFrameThenInvokeCallback(Action Callback = null) 
    {
        yield return null;
        yield return null;

        Callback?.Invoke();
    }
}
