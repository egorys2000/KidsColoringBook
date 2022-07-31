using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

using Vectrosity;

[System.Serializable]
public class LevelInfo
{
    public Color32[] Colors = null;
    public float[] Widths = null;

    [SerializeField]
    private List<LightVector2> _points = new List<LightVector2>();
    public List<LightVector2> Points
    {
        get
        {
            return _points;
        }

        set
        {
            _points = value;
        }
    }

    public readonly string LevelName;

    [SerializeField]
    private List<FragmentSnapshot> _levelFragments = new List<FragmentSnapshot>();
    public List<FragmentSnapshot> LevelFragments 
    {
        get => _levelFragments;
    }

    public LevelInfo(string levelName) 
    {
        LevelName = levelName;
    }

    public void Save() 
    {
        string fileName = this.Path();
        //BinarySerializer.Serialize<LevelInfo>(this, fileName);

        string json = JsonConvert.SerializeObject(this, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

        File.WriteAllText(fileName, json);
    }

    public static LevelInfo Load(string localPath) 
    {
        string fullPath = PathByName(localPath);

        if (!File.Exists(fullPath)) return null;

        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<LevelInfo>(json);
        //return BinarySerializer.Deserialize<LevelInfo>(PathByName(localPath));
    }

    public string Path()
    {
        return ContentManager.persistentDataPath + "/" + LevelName + ".json";
    }

    public static string PathByName(string name) 
    {
        return ContentManager.persistentDataPath + "/" + name + ".json";
    }

    public void DebugPrintFragments()
    {
        int i = 0;
        foreach (FragmentSnapshot fragment in LevelFragments) 
        {
            string print = "Fragment " + i.ToString() + ":";
            foreach (Vector2Int segment in fragment.Segment)
                print += segment.ToString() + "\n";
            Debug.Log(print);
            i++;
        }
    }
}

[System.Serializable]
public class LightVector2 
{
    private int _x, _y;

    public float x { get => (float)_x; }
    public float y { get => (float)_y; }

    public LightVector2(float x, float y) 
    {
        this._x = (int)x;
        this._y = (int)y;
    }

    public static implicit operator Vector2(LightVector2 lv) => new Vector2(lv.x, lv.y);
    public static explicit operator LightVector2(Vector2 v) => new LightVector2(v.x, v.y);
}

/// <summary>
/// Fragment line is a trim of global line, cut by [StartIdx; EndIdx]
/// </summary>
[System.Serializable]
public class FragmentSnapshot
{
    /// <summary>
    /// Segment = Vector2(a; b) means segment of indeces [a; b] trimmed from whole line belongs to this specific fragment. 
    /// Several connectivities components possible
    /// </summary>
    private List<Vector2Int> _segment;
    public List<Vector2Int> Segment 
    {
        get => _segment;
    }

    public bool Belongs(int idx) 
    {
        if (Segment.Count == 0) return false;

        foreach (Vector2Int connectivityComponent in Segment)
        {
            if (((idx >= connectivityComponent.x) && (idx <= connectivityComponent.y)))
            { return true; break; }
        }

        return false;
    }

    public FragmentSnapshot() 
    {
        _segment = new List<Vector2Int>();
    }

    public void AddSegment(int startIdx, int endIdx)
    {
        if (startIdx < 0)
            Debug.LogError("Index must be non-negative");

        if (endIdx < startIdx)
            return;
        else
            this._segment.Add(new Vector2Int(startIdx, endIdx));
    }
}