using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsRupturedComp))]
	public sealed class RuptureSystem : CustomEntitySetSystem
	{
		public RuptureSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			float rupturePercentage = Player.Get<RuptureDataComp>().Percentage;

			for (int i = 0; i < entities.Length; i++)
			{
				var entity = entities[i];

				ref readonly var position = ref entity.Get<PositionComp>();
				ref var oldPosition = ref entity.Get<OldPositionComp>();

				float distance = Vector2.Distance(position.Value, oldPosition.Value);

				if (distance <= 0.1f) //Ignore oscillations
					continue;

				entity.Get<HealthComp>().Current -= distance * rupturePercentage;
				//Debug.Log("Health: " + entity.Get<HealthComp>().Current);
				oldPosition.Value = position.Value;
			}
		}
	}
}