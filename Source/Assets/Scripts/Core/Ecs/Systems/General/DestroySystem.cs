using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RoguelikeEcs.Core
{
	[With(typeof(DestroyComp))]
	public sealed class DestroySystem : AEntitySetSystem<float>
	{
		public DestroySystem(World world) : base(world, true)
		{
			world.Subscribe((in KillAllEnemiesEvent message) =>
			{
				foreach (var entity in world.GetEntities().With<IsEnemy>().AsSet().GetEntities())
					entity.Set(new DestroyComp() { Internal = true });
			});

			world.Subscribe((in DestroyAllProjectilesEvent message) =>
			{
				foreach (var entity in world.GetEntities().With<IsProjectileComp>().AsSet().GetEntities())
					entity.Set(new DestroyComp() { Internal = true });
			});
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var score = ref World.Get<ScoreComp>();
			ref var killStreak = ref World.Get<KillStreakComp>();
			ref var deadEnemies = ref World.Get<DeadEnemiesListComp>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref var destroy = ref entity.Get<DestroyComp>();
				if (destroy.Counter-- > 0) //Delays destruction by one frame, needed for after death systems to work
					continue;

				if (entity.Has<IsEnemy>() && !destroy.Internal)
				{
					killStreak.Refresh();
					score.Kills++;
					if (entity.Has<IsBossEnemy>())
						score.BossesKilled++;
					else if (!entity.Has<IsNecromancerEnemy>()) //Ignore bosses and necromancers
						deadEnemies.Value.Add(new DeadEnemiesListComp.DeadEnemyInfo
							{ EnemyType = entity.Get<EnemyTypeComp>().Value, Position = entity.Get<PositionComp>().Value });
				}

				if (entity.Has<HealthBarComp>())
				{
					if (!Core.IsTestMode)
						Object.Destroy(entity.Get<HealthBarComp>().Transform.gameObject);
					else
						Object.DestroyImmediate(entity.Get<HealthBarComp>().Transform.gameObject);
				}

				if (entity.TryGet(out GameObjectComp goComp)) //Need this down because of enemy ranged attacks are entities
				{
					if (!Core.IsTestMode)
						Object.Destroy(goComp.Value);
					else
						Object.DestroyImmediate(goComp.Value);
				}

				entity.Dispose();
			}
		}
	}
}