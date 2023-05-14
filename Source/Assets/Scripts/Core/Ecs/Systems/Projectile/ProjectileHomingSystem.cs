using System;
using DefaultEcs;
using DefaultEcs.System;
using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsHomingProjectile))]
	public sealed class ProjectileHomingSystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _enemyEntitySet;

		private float _homeTimer;

		public ProjectileHomingSystem(World world) : base(world)
		{
			_enemyEntitySet = world.GetEntities().With<IsEnemy>()
				.Without<IsGhostEnemy>().Without<DoDeflectProjectilesComp>().Without<IsSleeping>()
				.AsSet();
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_homeTimer += state;
			if (_homeTimer < 0.05f)
				return;

			_homeTimer = 0;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var target = ref entity.Get<TargetComp>();
				if (target.Value == default || !target.Value.IsAlive)
				{
					//TODO Very not performant, refactor. Maybe box collide X range, and find the closest (if any) enemy.
					float shortestDistance = float.MaxValue;
					Entity closestEnemy = default;
					var position = entity.Get<PositionComp>().Value;
					foreach (var enemy in _enemyEntitySet.GetEntities())
					{
						float distanceToEnemy = Vector2.Distance(position, enemy.Get<PositionComp>().Value);
						if (distanceToEnemy < shortestDistance)
						{
							shortestDistance = distanceToEnemy;
							closestEnemy = enemy;
							if (shortestDistance < 10)
								break;
						}
					}

					target.Value = closestEnemy;
				}

				if (target.Value == default)
					continue;

				entity.Get<VelocityComp>().Direction =
					(target.Value.Get<PositionComp>().Value - entity.Get<PositionComp>().Value).normalized;
			}

			_enemyEntitySet.Complete();
		}
	}
}