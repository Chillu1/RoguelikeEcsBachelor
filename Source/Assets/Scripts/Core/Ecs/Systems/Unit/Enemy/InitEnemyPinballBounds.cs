using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RoguelikeEcs.Core
{
	[WhenAdded(typeof(IsPinballEnemy))]
	public sealed class InitEnemyPinballBounds : CustomEntitySetSystem
	{
		public InitEnemyPinballBounds(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			for (int i = 0; i < entities.Length; i++)
			{
				//Random directions: 0.5 to 1. -0.5 to -1
				float x = Random.Range(0, 2) == 0 ? Random.Range(0.5f, 1f) : Random.Range(-1f, -0.5f);
				float y = Random.Range(0, 2) == 0 ? Random.Range(0.5f, 1f) : Random.Range(-1f, -0.5f);
				entities[i].Get<VelocityComp>().Direction = new Vector2(x, y);

				//Old random direction into one of the 4 corners, so: (-1, -1), (-1, 1), (1, -1), (1, 1)
				//entities[i].Get<VelocityComp>().Direction = new Vector2(Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(0, 2) == 0 ? -1 : 1);
			}
		}
	}
}