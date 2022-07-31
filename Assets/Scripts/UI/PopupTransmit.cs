using System;
using UnityEngine;

public class PopupTransmit : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupPrefab, _screenToClose;

    private Action _openAnimation, _closeAnimation;

    public void OpenPrefabSPD()
    {
        if (_screenToClose != null)
            _screenToClose.SetActive(false);
        _popupPrefab.SetActive(true);
        _openAnimation?.Invoke();
    }

    public void ClosePrefabSPD()
    {
        if (_screenToClose != null)
            _screenToClose.SetActive(true);
        _popupPrefab.SetActive(false);
        _closeAnimation?.Invoke();
    }

    public void SetNewPrefabSPD(GameObject Prefab)
    {
        _popupPrefab = Prefab;
    }

}