using System;
using DefaultEcs;
using DefaultEcs.System;
using Object = UnityEngine.Object;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsProjectileComp))]
	public sealed class ProjectileBoundsSystem : AEntitySetSystem<float>
	{
		private readonly float _projectileMinX;
		private readonly float _projectileMaxX;
		private readonly float _projectileMinY;
		private readonly float _projectileMaxY;

		public ProjectileBoundsSystem(World world, Bounds bounds) : base(world)
		{
			_projectileMinX = bounds.ProjectileMin.x;
			_projectileMaxX = bounds.ProjectileMax.x;
			_projectileMinY = bounds.ProjectileMin.y;
			_projectileMaxY = bounds.ProjectileMax.y;
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			ref var score = ref World.Get<ScoreComp>();

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				ref readonly var position = ref entity.Get<PositionComp>();

				if (position.Value.x < _projectileMinX
				    || position.Value.x > _projectileMaxX
				    || position.Value.y < _projectileMinY
				    || position.Value.y > _projectileMaxY)
				{
					if (entity.Get<PiercingComp>().EntitiesHit.Count == 0)
						score.MissedProjectiles++;

					entity.Set<DestroyComp>();
				}
			}
		}
	}
}