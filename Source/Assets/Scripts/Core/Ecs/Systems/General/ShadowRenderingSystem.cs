using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(ShadowRendererComp))]
	public sealed class ShadowRenderingSystem : AEntitySetSystem<float>
	{
		private float _slowedTimer;

		public ShadowRenderingSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			_slowedTimer += state * 0.1f;

			for (int i = 0; i < entities.Length; i++)
			{
				ref readonly var entity = ref entities[i];

				var transform = entity.Get<ShadowRendererComp>().Value.transform;
				float ping = Mathf.PingPong(_slowedTimer, 0.05f);

				transform.localScale = Vector3.one * (1 + ping);
				transform.localPosition = new Vector3(-0.025f + ping, -0.775f + ping, 0);
			}
		}
	}
}