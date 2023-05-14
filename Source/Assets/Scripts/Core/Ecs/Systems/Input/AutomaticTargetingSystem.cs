using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp))]
	public sealed class AutomaticTargetingSystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _enemyEntitySet;

		public AutomaticTargetingSystem(World world) : base(world)
		{
			_enemyEntitySet = world.GetEntities().With<IsEnemy>().AsSet();
		}

		protected override void Update(float state, in Entity entity)
		{
			if (!World.Get<UpgradesComp>().AutomaticAim)
				return;

			ref readonly var position = ref entity.Get<PositionComp>();
			ref readonly var velocity = ref entity.Get<VelocityComp>();

			float speed = (velocity.Speed + entity.Get<StatsComp>().ExtraMoveSpeed) *
			              entity.Get<MultiplierComp>().BaseMultipliers[(int)StatType.MoveSpeed];
			var destinationPosition = position.Value + velocity.Direction * (speed * state);

			float closestDistance = float.MaxValue;
			Vector2 closestPosition = Vector2.zero;
			foreach (ref readonly var enemy in _enemyEntitySet.GetEntities())
			{
				ref readonly var enemyPosition = ref enemy.Get<PositionComp>();
				//ref readonly var enemyVelocity = ref enemy.Get<VelocityComp>();
				float enemyScaleMagnitude = enemy.Get<ScaleComp>().Magnitude;

				//TODO Enemy scale isn't used entirely correct here (probably)
				//TODO The higher enemy velocity, the less likely it will be targeted (chargers, and pinball enemies)
				float distance = Vector2.Distance(destinationPosition, enemyPosition.Value) / enemyScaleMagnitude;
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestPosition = enemyPosition.Value;
				}
			}

			if (closestDistance != float.MaxValue)
			{
				entity.Get<PlayerMouseClickComp>().Position = closestPosition;
				World.Get<MousePositionComp>().Value = closestPosition;
			}

			_enemyEntitySet.Complete();
		}
	}
}