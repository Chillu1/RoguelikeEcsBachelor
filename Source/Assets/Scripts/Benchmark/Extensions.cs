using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoguelikeEcs.Benchmark
{
	public static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetMagnitude(this Vector2 v)
		{
			return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetMagnitude(this Vector3 v)
		{
			return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 GetNormalized(this Vector2 v)
		{
			float magnitude = v.GetMagnitude();
			return magnitude == 0 ? Vector2.zero : new Vector2(v.x / magnitude, v.y / magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetNormalized(this Vector3 v)
		{
			float magnitude = v.GetMagnitude();
			return magnitude == 0 ? Vector3.zero : new Vector3(v.x / magnitude, v.y / magnitude, v.z / magnitude);
		}
	}
}