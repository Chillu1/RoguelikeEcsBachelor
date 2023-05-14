using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class SpawnArea
	{
		public Area[] Areas { get; }

		private readonly Bounds _bounds;

		public SpawnArea(Bounds bounds)
		{
			_bounds = bounds;
			Areas = new Area[4];
		}

		public void SetSpawnArea(int index, Vector2 min, Vector2 max)
		{
			Areas[index] = new Area(min, max);
		}

		public Vector2 GetRandomPosition()
		{
			var area = Areas[Random.Range(0, Areas.Length)];
			return new Vector2(Random.Range(area.Min.x, area.Max.x), Random.Range(area.Min.y, area.Max.y));
		}

		public Vector2 GetRandomPositionOnEdge()
		{
			//bool isHorizontal = Random.Range(0, 2) == 0;
			bool isPositive = Random.Range(0, 2) == 0;
			float x = isPositive ? _bounds.Max.x : _bounds.Min.x;
			float y = Random.Range(_bounds.Min.y, _bounds.Max.y);
			return new Vector2(x, y);
		}

		public Vector2 GetRandomPositionInBounds(Vector2 playerPosition)
		{
			int maxTries = 10;
			do
			{
				var position = new Vector2(Random.Range(_bounds.Min.x, _bounds.Max.x), Random.Range(_bounds.Min.y, _bounds.Max.y));
				if (Vector2.Distance(position, playerPosition) > 100f)
					return position;
				maxTries--;
			} while (maxTries > 0);

			Debug.LogError("Can't find position in bounds not next to player");
			return Vector2.zero;
		}

		public readonly struct Area
		{
			public Vector2 Min { get; }
			public Vector2 Max { get; }

			public Area(Vector2 min, Vector2 max)
			{
				Min = min;
				Max = max;
			}
		}
	}
}