using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CutImage : MonoBehaviour
{
    private Texture2D[] _piecesTextures;
    [SerializeField] private Image _displayImage;

    public const int xPieces = 3, yPieces = 2;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);

        BuildFragments(xPieces, yPieces, Content.ActiveLevelIconFullPath);

        string localPathToJson = Content.ActiveLevelShortName;

        if (!File.Exists(LevelInfo.PathByName(localPathToJson)))
            InitializeLevelData();
        else
        { 
            _levelInfo = LevelInfo.Load(localPathToJson);
            SetFirstIdxToLastIdxFromPreviousSession();
        }

        DrawLinesMouse.Get.RestoreWholeLine(_levelInfo);
        SetFragment(0);

        void SetFirstIdxToLastIdxFromPreviousSession() 
        {
            int maxIdx = 0;
            foreach (FragmentSnapshot fragment in _levelInfo.LevelFragments)
            {   
                if(fragment.Segment.Count > 0)
                    if (fragment.Segment[fragment.Segment.Count - 1].y >= maxIdx)
                    maxIdx = fragment.Segment[fragment.Segment.Count - 1].y + 1;
            }
            DrawLinesMouse.Get.LineFirstDotIdx = maxIdx;
        }
    }

    private static Texture2D PngToTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //  this will auto-resize the texture dimensions.
        }
        return tex;
    }

    public void BuildFragments(int x_pieces, int y_pieces, string pathTopPng)
    {
        _piecesTextures = new Texture2D[x_pieces * y_pieces];
        Texture2D sourceTexture = Content.OpenedLevelImage;

        for (int x = 0; x < x_pieces; x++)
        {
            for (int y = 0; y < y_pieces; y++)
            {
                int index = x * y_pieces + y;

                _piecesTextures[index] = new Texture2D(sourceTexture.width / x_pieces, sourceTexture.height / y_pieces);
                var pixels =  sourceTexture.GetPixels
                    (sourceTexture.width / x_pieces * x, sourceTexture.height / y_pieces * y, sourceTexture.width / x_pieces, sourceTexture.height / y_pieces);
                _piecesTextures[index].SetPixels(pixels);
                _piecesTextures[index].Apply();
            }
        }
    }

    private static LevelInfo _levelInfo;
    public static FragmentSnapshot CurrentFragment { get => _levelInfo.LevelFragments[_fragment]; }

    private void InitializeLevelData() 
    {
        _levelInfo = new LevelInfo(Content.ActiveLevelShortName);

        for (int i = 0; i < xPieces * yPieces; i++) 
        {
            //TODO: load levels from saves
            _levelInfo.LevelFragments.Add(new FragmentSnapshot());
        }
    }

    public static void SaveFragment() 
    {
        _levelInfo.LevelFragments[_fragment] =
                DrawLinesMouse.Get.FragmentData(_levelInfo.LevelFragments[_fragment]); //save what's drawn on this fragment
        _levelInfo.Colors = DrawLinesMouse.Get.GetLineColors(); //save what's drawn on this fragment

        List<LightVector2> newPoints = new List<LightVector2>();
        foreach (var point in DrawLinesMouse.Get.GetLinePoints())
            newPoints.Add((LightVector2)point);
        _levelInfo.Points = newPoints; //save what's drawn on this fragment

        _levelInfo.Save(); //save what's drawn on this fragment
    }

    private void SetFragment(int newFragment) 
    {
        SaveFragment();

        DrawLinesMouse.Get.UncoverNeededFragment(_levelInfo, newFragment); //load new fragment
                                                                           //DrawLinesMouse.Get.GetLineColors();
        _fragment = newFragment;

        Rect rec = new Rect(0, 0, _piecesTextures[0].width, _piecesTextures[0].height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        _displayImage.sprite = Sprite.Create(_piecesTextures[_fragment], rec, pivot);
    }

    [SerializeField]
    private GameObject _nextGrayIcon, _previousGrayIcon;

    private static int _fragment;
    public void NextFrangent()
    {
        if (_fragment < xPieces * yPieces - 1)
            SetFragment(_fragment + 1);

        if (_fragment == xPieces * yPieces - 1)
            _nextGrayIcon.SetActive(true);
        else
            _nextGrayIcon.SetActive(false);

        if (_fragment == 0)
            _previousGrayIcon.SetActive(true);
        else
            _previousGrayIcon.SetActive(false);
    }

    public void PreviousFrangent()
    {
        if (_fragment > 0) SetFragment(_fragment - 1);

        if (_fragment == xPieces * yPieces - 1)
            _nextGrayIcon.SetActive(true);
        else
            _nextGrayIcon.SetActive(false);

        if (_fragment == 0)
            _previousGrayIcon.SetActive(true);
        else
            _previousGrayIcon.SetActive(false);
    }
}