using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsEnemy), typeof(RendererComp))]
	[Without(typeof(IsSleeping))]
	public sealed class EnemyRenderingSystem : CustomEntitySetSystem
	{
		private readonly Quaternion _lowRotation, _highRotation;

		private float _timer;

		public EnemyRenderingSystem(World world) : base(world)
		{
			_lowRotation = Quaternion.Euler(0, 0, -15);
			_highRotation = Quaternion.Euler(0, 0, 15);
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_timer += state * 0.8f;
			float slowedTimer = _timer * 0.5f;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];
				var go = entity.Get<RendererComp>().Child;

				if (entity.Has<VelocityComp>())
				{
					float yRotation = 0;
					if (entity.Get<VelocityComp>().Direction.x < 0)
						yRotation = 180;

					go.transform.rotation = Quaternion.Lerp(_lowRotation, _highRotation, Mathf.PingPong(_timer, 1)) *
					                        Quaternion.Euler(0, yRotation, 0); //TODO Inefficient
				}

				go.transform.localScale = Vector3.one * (1 + Mathf.PingPong(slowedTimer, 0.1f));
			}
		}
	}
}