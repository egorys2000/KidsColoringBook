using UnityEngine;
using System;
using Vectrosity;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ConstructDrawing : MonoBehaviour
{
	[SerializeField]
	private CarouselGObject _carouselGO;
	[SerializeField]
	private Material _material;

	async void Start()
	{
		SetupLine();
		if (_carouselGO.Name != null)
		{
			TuneGeometry();

			long GetTime() // in milliseconds 
			{
				return (long)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			}
			long milliseconds = GetTime();
			var loadedLine = await Task.Run(() => LevelInfo.Load(_carouselGO.Name));

			RestoreWholeLine(loadedLine);
		}		
	}

	public Texture2D lineTex;
	public int maxPoints = 100000;
	public float additionalLineWidth = +.5f;

	private const float lineWidthSensitivity = 50f, minLineWidth = 5.0f;

	private float LineWidth
	{
		get => minLineWidth + additionalLineWidth * lineWidthSensitivity;
	}

	public bool useEndCap = false;
	public Texture2D capLineTex;
	public Texture2D capTex;
	public float capLineWidth = 20.0f;
	// If line3D is true, the line is drawn in the scene rather than as an overlay. Note that in this demo, the line will look the same
	// in the game view either way, but you can see the difference in the scene view.
	private bool line3D = false;
	public float distanceFromCamera = 1.0f;

	private VectorLine _line;
	private RectTransform _rT;

	[SerializeField]
	private Vector2 _leftBottomCutoff, _rightTopCutoff;

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
			useLineWidth = LineWidth / 5f;
		}

		_line = new VectorLine("DrawnLine", new List<Vector2>(), tex, useLineWidth, LineType.Continuous, Joins.Fill);
		_line.smoothWidth = true;
		_rT = _line.rectTransform;

		//_line.layer = LayerMask.NameToLayer("TransparentFX");
		_rT.transform.SetParent(gameObject.transform);
		_rT.anchoredPosition = Vector2.zero;
		if (_material != null)
			_line.material = _material;

		_line.endPointsUpdate = maxPoints;  // Optimization for updating only the last couple points of the line, and the rest is not re-computed
		if (useEndCap)
		{
			_line.endCap = "RoundCap";
		}
	}


	private float _XscaleFactor, _YscaleFactor, _Xr0Tuned, _Yr0Tuned;
	private float _fragmentXShift, _fragmentYShift;

	public void TuneGeometry() // linearly interpolates coefficients of Screen resolution
	{
		//Debug.Log(Screen.width.ToString() + ", " + Screen.height.ToString());

		_YscaleFactor = (0.00015503f * Screen.height + 0.5905f) * 1.04f;
		_XscaleFactor = _YscaleFactor * (-0.207265f * Screen.width / Screen.height + 1.17021043141f) * 1f;

		_Yr0Tuned = 0.048276f * Screen.height + 125f;
		_Xr0Tuned = _Yr0Tuned * (0.02731000f * Screen.width / Screen.height + 0.5625706f);

		_fragmentYShift = 0.1276f * Screen.height + 300f;

		if (Screen.height < 900)
			_fragmentYShift -= 100f;
		_fragmentXShift = _fragmentYShift * (0.22816f * Screen.width / Screen.height + 0.5147f);
	}

	public void RestoreWholeLine(LevelInfo oldLine)
	{
		if (oldLine == null) return;

		Vector2 r0 = new Vector2(+_Xr0Tuned * _XscaleFactor, _Yr0Tuned * _YscaleFactor);
		Vector2 RescaleVector(Vector2 point) 
		{
			//point = new Vector2(point.x / CutImage.xPieces, point.y / CutImage.yPieces);
			//point = new Vector2(point.x * _XscaleFactor, point.y * _YscaleFactor);
			//point += r0;
			point = new Vector2(point.x / CutImage.xPieces * _XscaleFactor, point.y / CutImage.yPieces * _YscaleFactor) + r0;
			return point;
		}

		for (int i = 0; i < oldLine.Points.Count; i++)
		{
			Vector2 point = oldLine.Points[i];
			_line.points2.Add(RescaleVector(point));
		}
		
		_line.SetColors(oldLine.Colors);

		//Debug.Log("Points: " + oldLine.Points.Count.ToString());

		for (int i = 0; i < CutImage.xPieces * CutImage.yPieces; i++)
			ShiftNeededFragment(oldLine, i);
	}

	public void ShiftNeededFragment(LevelInfo oldLine, int fragmentNumber)
	{
		if (oldLine == null) return;

		int pointCount = _line.points2.Count;

		for (int i = 0; i < pointCount; i++)
		{
			int xIdx = fragmentNumber % CutImage.yPieces;
			int yIdx = fragmentNumber / CutImage.yPieces;
			if (oldLine.LevelFragments[fragmentNumber].Belongs(i))
			{
				_line.points2[i] += new Vector2(_fragmentXShift * yIdx * _XscaleFactor, _fragmentYShift * xIdx * _YscaleFactor);
				Vector2 p = _line.points2[i];

				// manual UI mask //TODO: make normal mask via Editor
				if (p.x < _leftBottomCutoff.x || p.x > _rightTopCutoff.x || p.y < _leftBottomCutoff.y || p.y > _rightTopCutoff.y)
					_line.SetColor(Color.clear, i);
			}
		}

		_line.Draw();
	}

	public void ClearScreen()
	{
		_line.Draw();
		_line.points2.Clear();
	}

	public void ChangeLineWidth(float newWidth)
	{
		additionalLineWidth = newWidth;
	}
}