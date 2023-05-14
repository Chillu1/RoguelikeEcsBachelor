using DefaultEcs;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[With(typeof(PlayerMouseClickComp))]
	public class ResetInputSystem : AEntitySetSystem<float>
	{
		public ResetInputSystem(World world) : base(world)
		{
		}

		protected override void Update(float state, in Entity entity)
		{
			entity.Get<PlayerMouseClickComp>().IsClicked = false;
		}
	}
}