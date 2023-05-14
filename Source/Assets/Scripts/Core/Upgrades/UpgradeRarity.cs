using System;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public enum UpgradeRarity
	{
		None,
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary
	}

	public static class UpgradeRarityExtensions
	{
		public static float GetPrice(this UpgradeRarity rarity)
		{
			switch (rarity)
			{
				case UpgradeRarity.Common:
					return 50f;
				case UpgradeRarity.Uncommon:
					return 100f;
				case UpgradeRarity.Rare:
					return 150f;
				case UpgradeRarity.Epic:
					return 250f;
				case UpgradeRarity.Legendary:
					return 500f;
				default:
					Debug.LogError($"Unknown rarity: {rarity}");
					return GetPrice(UpgradeRarity.Rare);
			}
		}

		public static Color GetColor(this UpgradeRarity rarity)
		{
			switch (rarity)
			{
				case UpgradeRarity.Common:
					return Color.gray.Alpha(0.5f);
				case UpgradeRarity.Uncommon:
					return new Color(0.5f, 1f, 0.5f, 0.5f);
				case UpgradeRarity.Rare:
					return new Color(0.5f, 0.5f, 1f, 0.5f);
				case UpgradeRarity.Epic:
					return new Color(1f, 0.5f, 1f, 0.5f);
				case UpgradeRarity.Legendary:
					return new Color(1f, 0.92156863f, 0.015686275f, 0.5f);
				default:
					Debug.LogError($"Unknown rarity: {rarity}");
					return GetColor(UpgradeRarity.Rare);
			}
		}
	}
}