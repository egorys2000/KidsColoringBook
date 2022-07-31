using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselElement : ICarouselElement
{
    private Texture2D _imageTexture = null;

    private readonly string _name, _filePath;

    public GameObject GO;

    public CarouselElement(string name, string fileLocalPath) 
    {
        this._name = name;
        this._filePath = fileLocalPath;
    }

    public void Initialize() { }
    public async Task<Texture2D> GetTexture() 
    {
        Texture2D tex = null;

        if (!System.IO.File.Exists(Parser.FullPath(_filePath)))
        { await ContentManager.Get.DownloadFile(_filePath); }

        if (!System.IO.File.Exists(Parser.FullPath(_filePath) )) return null;     //if for some resons couldn't download file, return null
        
        var fileData = System.IO.File.ReadAllBytesAsync(Parser.FullPath(_filePath));

        tex = new Texture2D(2, 2);
        tex.LoadImage(await fileData); // this will auto-resize the texture dimensions.
        _imageTexture = tex;

        return TrimTexture(tex);
    }

    /// <summary>
    /// Defines rectangle of non-transparent subarea
    /// </summary>
    /// <param name="texture"> source texture </param>
    /// <param name="x0"> lower-left corner x-coord </param>
    /// <param name="y0"> lower-left corner y-coord </param>
    /// <param name="width"> width </param>
    /// <param name="height"> height </param>
    private void GetCropCorners(Texture2D texture, out int x0, out int y0, out int width, out int height) 
    {
        long GetTime() // in milliseconds 
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        long milliseconds = GetTime();

        x0 = 0;
        y0 = 0;
        width = texture.width;
        height = texture.height;

        var data = texture.GetRawTextureData<Color32>();

        int step = 4;
        bool transpaperntRow(int column) 
        {
            bool transparent = true;
            for (int row = 0; row < texture.height; row += step)
                transparent = transparent && (data[row * texture.width + column] == Color.clear);

            return transparent;
        }

        bool transpaperntColumn(int row)
        {
            bool transparent = true;
            for (int column = 0; column < texture.height; column += step)
                transparent = transparent && (data[row * texture.width + column] == Color.clear);

            return transparent;
        }

        for (int x = 0; x < texture.width; x++)
        {
            if (transpaperntRow(x)) x0++;
            else break;
        }
        width -= x0;
        for (int x = texture.width; x > x0; x--)
        {
            if (transpaperntRow(x)) width--;
            else break;
        }

        for (int y = 0; y < texture.height; y++)
        {
            if (transpaperntColumn(y)) y0++;
            else break;
        }
        height -= y0;
        for (int y = texture.height; y > y0; y--)
        {
            if (transpaperntColumn(y)) height--;
            else break;
        }

        Debug.Log("Cropped with texture.getPixel for " + (GetTime() - milliseconds).ToString() + " ms");
    }

    private Texture2D TrimTexture(Texture2D texture) 
    {
        int newWidth, newHeight, x0, y0;

        GetCropCorners(texture, out x0, out y0, out newWidth, out newHeight);

        Color[] colors = texture.GetPixels(x0, y0, newWidth, newHeight);
        Texture2D croppedTexture = new Texture2D(newWidth, newHeight);
        croppedTexture.SetPixels(colors);
        croppedTexture.Apply();

        return croppedTexture;
    }

    public void Dispose() { }

    public string Name { get => _name; }
    public string Link { get => _filePath; }
}

public interface ICarouselElement 
{
    public void Initialize();
    public Task<Texture2D> GetTexture();
    public void Dispose();

    public string Name { get; }
    public string Link { get; }
}
