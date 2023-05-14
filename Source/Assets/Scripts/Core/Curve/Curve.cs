using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public class Curve
	{
		private List<CurvePoint> Points { get; }
		private readonly bool _negativeAllowed;

		public Curve(List<CurvePoint> curvePoints, bool monotonic = true, bool negativeAllowed = false)
		{
			Points = curvePoints;
			_negativeAllowed = negativeAllowed;
			if (monotonic)
				ValidateCurve();
		}

		private void ValidateCurve()
		{
			for (int i = 0; i < Points.Count - 1; i++)
			{
				CurvePoint point = Points[i];

				if (point.X > Points[i + 1].X)
				{
					Debug.LogError("Curve data is invalid");
					break;
				}
			}
		}

		public float Evaluate(float x)
		{
			float negativeMultiplier = 1.0f;
			if (x < 0)
			{
				if (!_negativeAllowed)
					Debug.LogError($"Negative value: {x} in curve, without negative flag");
				x = Math.Abs(x);
				negativeMultiplier = -1.0f;
			}

			if (x <= Points[0].X) //Min
			{
				return Points[0].Y * negativeMultiplier;
			}

			if (x >= Points[Points.Count - 1].X) //Max
			{
				return Points[Points.Count - 1].Y * negativeMultiplier;
			}

			CurvePoint firstCurvePoint = Points[0];
			CurvePoint lastCurvePoint = Points[Points.Count - 1];
			for (int i = 0; i < Points.Count; i++)
			{
				if (x <= Points[i].X)
				{
					lastCurvePoint = Points[i];
					if (i > 0)
					{
						firstCurvePoint = Points[i - 1];
					}

					break;
				}
			}

			float t = (x - firstCurvePoint.X) / (lastCurvePoint.X - firstCurvePoint.X);
			return (firstCurvePoint.Y + (lastCurvePoint.Y - firstCurvePoint.Y) * Clamp01(t)) * negativeMultiplier;
		}

		private static float Clamp01(float value)
		{
			if (value < 0.0f)
				return 0.0f;
			return value > 1.0f ? 1f : value;
		}
	}
}