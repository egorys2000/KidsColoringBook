using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SwitchHandler : MonoBehaviour
{
    private bool _switchState = true;

    [SerializeField]
    private GameObject switchBtn;
    [SerializeField]
    private Image _backgroundImg;

    private float _colorMultiplier = .7f;

    public void OnSwitchButtonClicked()
    {
        switchBtn.transform.DOLocalMoveX(-switchBtn.transform.localPosition.x, 0.2f)
            .OnComplete(() =>
            {
                _switchState = !_switchState;

                if (!_switchState)
                    _backgroundImg.color = _backgroundImg.color * _colorMultiplier;
                else
                    _backgroundImg.color = _backgroundImg.color / _colorMultiplier;
            }
            );        

        SettingsUIManager.Get.ToggleMusic(_switchState);
    }

}