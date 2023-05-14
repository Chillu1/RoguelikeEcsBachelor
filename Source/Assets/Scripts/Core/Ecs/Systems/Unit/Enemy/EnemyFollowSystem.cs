using System;
using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(VelocityComp))]
	[Without(typeof(IsChargeEnemy), typeof(DestinationPositionComp), typeof(IsSleeping), typeof(IsMapEdgeEnemy),
		typeof(IsDistanceFollowEnemy), typeof(IsPinballEnemy))]
	public sealed class EnemyFollowSystem : CustomEntitySetSystem
	{
		public EnemyFollowSystem(World world) : base(world, SystemParameters.UsePlayer, interval: 0.2f)
		{
		}

		protected override void Update(float delta, ReadOnlySpan<Entity> entities)
		{
			var playerPosition = Player.Get<PositionComp>().Value;
			ref readonly var playerVelocity = ref Player.Get<VelocityComp>();
			var playerDestination = playerPosition + playerVelocity.Direction * (playerVelocity.Speed * 20f * delta);

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var position = ref entity.Get<PositionComp>();

				var normalized = entity.Has<IsSmartFollowEnemy>()
					? (playerDestination - position.Value).normalized
					: (playerPosition - position.Value).normalized;

				entity.Get<VelocityComp>().Direction = normalized;
			}
		}
	}
}