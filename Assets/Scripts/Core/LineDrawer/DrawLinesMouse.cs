using UnityEngine;
using System;
using Vectrosity;
using System.Linq;
using System.Collections.Generic;

public class DrawLinesMouse : MonoBehaviour 
{
	private static DrawLinesMouse _instance;
	public static DrawLinesMouse Get 
	{
		get => _instance;
	}

	public bool ClickedOnUI = false;

	void Awake() 
	{
		_instance = this;

		SetupLine();
	}

	public Color ChosenColor;

	public Texture2D lineTex;
	public int maxPoints = 5000;
	public float additionalLineWidth = .5f;

	private const float lineWidthSensitivity = 50f, minLineWidth = 5.0f;
	private Touch touch;
	private float LineWidth 
	{
		get => minLineWidth + additionalLineWidth * lineWidthSensitivity;
	}

	public int minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
	public bool useEndCap = false;
	public Texture2D capLineTex;
	public Texture2D capTex;
	public float capLineWidth = 20.0f;
	// If line3D is true, the line is drawn in the scene rather than as an overlay. Note that in this demo, the line will look the same
	// in the game view either way, but you can see the difference in the scene view.
	private bool line3D = false;
	public float distanceFromCamera = 1.0f;
	
	private VectorLine _line;
	private Vector3 _previousPosition;
	private int _sqrMinPixelMove;

	private bool _canDraw = false;
	private bool _nextPointTransparent = false;

	[SerializeField]
	public bool CanErase = false;

	private void SetupLine() 
	{
		float useLineWidth;
		Texture2D tex;
		if (useEndCap)
		{
			VectorLine.SetEndCap("RoundCap", EndCap.Mirror, capLineTex, capTex);
			tex = capLineTex;
			useLineWidth = capLineWidth;
		}
		else
		{
			tex = lineTex;
			useLineWidth = LineWidth;
		}

		_line = new VectorLine("DrawnLine", new List<Vector2>(), tex, useLineWidth, LineType.Continuous, Joins.Fill);
		_line.smoothWidth = true;
		_line.endPointsUpdate = 2;  // Optimization for updating only the last couple points of the line, and the rest is not re-computed
		if (useEndCap)
		{
			_line.endCap = "RoundCap";
		}
		// Used for .sqrMagnitude, which is faster than .magnitude
		_sqrMinPixelMove = minPixelMove * minPixelMove;
	}

	[SerializeField]
	private float _eraserRadius;

	void Update () 
	{
		if (Input.touchCount > 0)
		{
			touch = Input.GetTouch(0);
		}

		if (CanErase)
		{
			if (Input.GetMouseButton(0))
				Erase(GetTouchPos(), _eraserRadius);
		}
		else
			DrawLine(GetTouchPos(), ChosenColor, LineWidth);

	}

	public void RestoreWholeLine(LevelInfo oldLine) 
	{
		ClearScreen();
		for (int i = 0; i < oldLine.Points.Count; i++)
		{
			_line.points2.Add(oldLine.Points[i]);
			_line.Draw();
		}
		_line.SetColors(oldLine.Colors);
		_line.Draw();
	}

	public void UncoverNeededFragment(LevelInfo oldLine, int fragmentNumber) 
	{
		if (oldLine == null) return;

		int pointCount = _line.points2.Count;
		
		for (int i = 0; i < pointCount; i++) 
		{
			if (oldLine.LevelFragments[fragmentNumber].Belongs(i))
			{
				_line.SetColor(oldLine.Colors[i], i);
			}
			else
			{
				//_line.SetColor(Color.black, i - 2, memorize: false);
				_line.SetColor(Color.clear, i - 1, memorize: false);
			}
		}
		
		foreach (FragmentSnapshot fragment in oldLine.LevelFragments)
			foreach (Vector2Int slice in fragment.Segment)
				_line.SetColor(Color.clear, (int)slice.y - 1, memorize: false);

		_line.Draw();
	}

	public Color32[] GetLineColors() 
	{
		T[] DeepCopy<T>(List<T> lst)
		{
			return lst.ToArray();
		}

		return DeepCopy<Color32>(_line.ColorsMemorized);
	}

	public List<Vector2> GetLinePoints()
	{
		T[] DeepCopy<T>(List<T> lst)
		{
			return lst.ToArray();
		}

		return DeepCopy<Vector2>(_line.points2).ToList();
	}

	private int _lineFirstDotIdx = 0;
	public int LineFirstDotIdx { get => _lineFirstDotIdx; set { _lineFirstDotIdx = value; } }
	/// <summary>
    /// Adds segment to FragmentSnapshot
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
	public FragmentSnapshot FragmentData(FragmentSnapshot lineSnapshot)
	{
		List<T> DeepCopy<T>(List<T> lst) 
		{
			return new List<T>(lst.ToArray());
		}

		int pointsAmount = _line.points2.Count;

		if (pointsAmount <= 1) { /*Debug.Log("Empty FrameSnapshot");*/ return lineSnapshot; }

		lineSnapshot.AddSegment(_lineFirstDotIdx, pointsAmount);

		//foreach(var print in lineSnapshot.Segment) Debug.Log(print);

		_lineFirstDotIdx = pointsAmount + 1;

		return lineSnapshot;
	}

	public void TakeEraser() 
	{
		CanErase = true;
	}

	[SerializeField] private RectTransform _drawZone;

	private void DrawInitialDot(Vector2 newPoint, Color ChosenColor, float LineWidth)
	{
		//line.points2.Clear();
		_line.Draw();

		_previousPosition = Input.mousePosition;

		_line.points2.Add(newPoint);

		int pointCount = _line.points2.Count;
		_line.SetWidth(LineWidth, pointCount - 1);
		_line.SetColor(Color.clear, pointCount - 1);

		_canDraw = true;
	}

	private void DrawContinuosLine(Vector2 newPoint, Color ChosenColor, float LineWidth)
	{
		_previousPosition = GetTouchPos();
		int pointCount;

		_line.points2.Add(newPoint);
		pointCount = _line.points2.Count;
		if (_nextPointTransparent)
		{
			_line.SetColor(Color.clear, pointCount - 3);
			_line.SetColor(Color.clear, pointCount - 2);
			_nextPointTransparent = false;
		}
		else
			_line.SetColor(ChosenColor, pointCount - 2);

		_line.SetWidth(LineWidth, pointCount - 1);
		_line.Draw();

		if (pointCount >= maxPoints)
		{
			_line.SetColor(ChosenColor, pointCount - 2);
			_canDraw = false;
		}
	}

	private void DrawFakeDot()
	{
		int pointCount;

		pointCount = _line.points2.Count;
		_line.SetColor(Color.clear, pointCount - 1);
		_canDraw = false;
		_nextPointTransparent = true;
	}

	public void DrawLine(Vector2 newPoint, Color ChosenColor, float LineWidth) 
	{
		if (ClickedOnUI) return;
		// Mouse button clicked, so start a new line
		if ((touch.phase == TouchPhase.Began))
		{
			DrawInitialDot(newPoint, ChosenColor, LineWidth);
		}
		// Mouse button held down and mouse has moved far enough to make a new point
		else 
		if ((touch.phase == TouchPhase.Moved && ((Vector3)touch.position - _previousPosition).sqrMagnitude > _sqrMinPixelMove && _canDraw))
		{
			float widthDeviation = UnityEngine.Random.Range(-LineWidth, +LineWidth) * 0.2f;
			DrawContinuosLine(newPoint, ChosenColor, LineWidth + widthDeviation);
		}
		else if (touch.phase == TouchPhase.Ended && _canDraw)
		{
			DrawFakeDot();
		}
	}

	public void Erase(Vector2 point, float radius) 
	{
		for(int i = 0; i < _line.points2.Count; i++)
		{
			if (((point - _line.points2[i]).sqrMagnitude < radius * radius) && 
					(CutImage.CurrentFragment.Belongs(i) || i > _lineFirstDotIdx))
			{
				_line.SetColor(Color.clear, i - 1);
			}
		}

		_line.Draw();
	}

	public void ClearScreen() 
	{
		//_line.ResetLine();
		_line.Draw();
		_line.points2.Clear();
	}

	public void ChangeLineWidth(float newWidth) 
	{
		additionalLineWidth = newWidth;
	}
	
	Vector3 GetTouchPos () 
	{
		/*var p = Input.mousePosition;
		if (line3D) {
			p.z = distanceFromCamera;
			return Camera.main.ScreenToWorldPoint (p);
		}
		return p;*/

		return touch.position;
	}
}