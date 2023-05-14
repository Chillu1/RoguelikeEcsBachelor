using UnityEngine;

namespace RoguelikeEcs.Core.Utilities
{
	public static class Utilities
	{
		public static float Lerp3(float a, float b, float c, float t)
		{
			return t < 0.5f ? Mathf.Lerp(a, b, t * 2) : Mathf.Lerp(b, c, (t - 0.5f) * 2);
		}
	}
}