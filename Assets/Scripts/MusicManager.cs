using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private List<AudioClip> _clip;

    private static MusicManager _instance;
    public static MusicManager Get 
    { get => _instance; }

    private bool _mustPlay = true; //responsible for Application focus/unfocus switching
    private bool _settingsToggle = true; //responsible for nusic toggle in settings

    public void TurnMusic() 
    {
        _settingsToggle = !_settingsToggle;
        if (!_settingsToggle) _source.Pause();
        else _source.Play();
    }

    void Awake()
    {
        if (_instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        _mustPlay = true;
        if (_settingsToggle) _source.Play();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        _mustPlay = false;
        _source.Pause();
    }

    void FixedUpdate()
    {
        if (!_source.isPlaying && _settingsToggle && _mustPlay)
        {
            _source.clip = _clip[Random.Range(0, _clip.Count - 1)];
            _source.Play();
        }
    }

}
