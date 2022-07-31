using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Slider))]
public class LoadSlider : MonoBehaviour
{
    private bool Loaded = false;

    private Slider ThisLoadSlider;

    [SerializeField] private string _sceneName;

    /// <summary>
    /// Updates LoadSlider bar and invokes callback after load
    /// </summary>

    private Action<string> Callback = (string _sceneName) =>
    {
        SceneManager.LoadScene(_sceneName);
    };

    void Start()
    {
        ThisLoadSlider = GetComponent<Slider>();
        StartCoroutine(LoaderProgress());
    }

    /// <summary>
    /// Updates LoadSlider bar and invokes callback after load
    /// </summary>
    private IEnumerator LoaderProgress()
    {
        while (!Loaded)
        {
            ThisLoadSlider.value += .03f / 4f; // illusion of loading

            yield return new WaitForSeconds(.025f);
            Loaded = ThisLoadSlider.value >= 1f;
        }

        Callback?.Invoke(_sceneName);
    }
}
