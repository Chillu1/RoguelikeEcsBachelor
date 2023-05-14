using System;
using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;

namespace RoguelikeEcs.Benchmark.ECS_Time
{
	public sealed class TimeSystem : AComponentSystem<float, IntervalComp>
	{
		public TimeSystem(World world) : base(world)
		{
		}

		public TimeSystem(World world, IParallelRunner runner) : base(world, runner, 1000)
		{
		}

		protected override void Update(float state, Span<IntervalComp> components)
		{
			for (int i = 0; i < components.Length; i++)
			{
				ref var interval = ref components[i];
				interval.Time += state;

				if (interval.Time >= interval.Interval)
				{
					interval.Time = 0;
					interval.Counter++;
				}
			}
		}
	}
}