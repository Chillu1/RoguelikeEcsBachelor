using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Benchmark.ECS_Follow
{
	[With(typeof(IsPlayerComp))]
	internal sealed class PlayerMovementSystem : AEntitySetSystem<float>
	{
		public PlayerMovementSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			float time = Time.time;

			for (int i = 0; i < entities.Length; i++)
			{
				entities[i].Get<PositionComp>().Value = new Vector2(
					Mathf.Sin(time) * 50,
					Mathf.Cos(time) * 50
				);
			}
		}
	}
}