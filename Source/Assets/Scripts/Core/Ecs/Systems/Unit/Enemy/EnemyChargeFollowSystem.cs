using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsChargeEnemy))]
	public sealed class EnemyChargeFollowSystem : CustomEntitySetSystem
	{
		private float _playerUpdateTimer;
		private Vector2 _playerPosition;
		private Vector2 _playerDestination;

		public EnemyChargeFollowSystem(World world) : base(world, SystemParameters.UsePlayer)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_playerUpdateTimer -= state;
			if (_playerUpdateTimer <= 0f)
			{
				_playerUpdateTimer = 0.25f;

				ref readonly var playerVelocity = ref Player.Get<VelocityComp>();
				var playerPosition = Player.Get<PositionComp>().Value;
				_playerPosition = playerPosition;
				_playerDestination = playerPosition + playerVelocity.Direction * (playerVelocity.Speed * 15f * state);
			}

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				if (entity.Get<ChargeTimerComp>().Timer > 0.5f)
					continue;

				//if player is close to the enemy, don't use player velocity direction
				entity.Get<VelocityComp>().Direction = Vector2.Distance(_playerPosition, entity.Get<PositionComp>().Value) < 100f
					? (_playerPosition - entity.Get<PositionComp>().Value).normalized
					: (_playerDestination - entity.Get<PositionComp>().Value).normalized;
				entity.Get<ChargeTimerComp>().Reset();
			}
		}
	}
}