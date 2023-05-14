using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeEcs.Core
{
	public struct HealthBarComp
	{
		public RectTransform Transform;
		public Image HealthBarLinesImage;
		public Image HealthBarFgImage;
		public Image HealthBarDamageTakenImage;

		public float PixelsPerUnitMultiplier;

		public float LastHealthPercent;
		public float DamageTakenTimer;
		public float LastMaxHealth;
	}
}