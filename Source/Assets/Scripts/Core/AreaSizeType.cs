using UnityEngine;

namespace RoguelikeEcs.Core
{
	public enum AreaSizeType
	{
		Small = -1,
		Medium = 0,
		Large
	}

	public static class AreaSizeTypeExtensions
	{
		public static Vector2 GetSize(this AreaSizeType sizeType)
		{
			switch (sizeType)
			{
				case AreaSizeType.Small:
					return new Vector2(350, 350);
				case AreaSizeType.Medium:
					return new Vector2(500, 500);
				case AreaSizeType.Large:
					return new Vector2(750, 750);
				default:
					return new Vector2(500, 500);
			}
		}

		public static float GetMultiplier(this AreaSizeType sizeType)
		{
			switch (sizeType)
			{
				case AreaSizeType.Small:
					return 0.7f;
				case AreaSizeType.Medium:
					return 1f;
				case AreaSizeType.Large:
					return 1.5f;
				default:
					return 1f;
			}
		}

		public static float GetPositionMultiplier(this AreaSizeType sizeType)
		{
			switch (sizeType)
			{
				case AreaSizeType.Small:
					return 0.66f;
				case AreaSizeType.Medium:
					return 1f;
				case AreaSizeType.Large:
					return 1.568f;
				default:
					return 1f;
			}
		}
	}
}