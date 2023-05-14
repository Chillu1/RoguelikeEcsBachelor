using System;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace ModifierSystemEcs.Benchmarks
{
	public class LoopBenchmark
	{
		private const int EntityCount = 1000;
		private const float DeltaTime = 0.0167f;

		private const int Iterations = 1000;

		private struct TimeComp
		{
			public float Value;
		}


		[With(typeof(TimeComp))]
		public sealed class ForLoopSystem : AEntitySetSystem<float>
		{
			public ForLoopSystem(World world) : base(world)
			{
			}

			protected override void Update(float state, ReadOnlySpan<Entity> entities)
			{
				for (int i = 0; i < entities.Length; i++)
				{
					ref readonly var entity = ref entities[i];
					entity.Get<TimeComp>().Value += state;
				}
			}
		}

		[Test, Performance]
		public void ForLoop()
		{
			var world = new World();
			for (int i = 0; i < EntityCount; i++)
			{
				var entity = world.CreateEntity();
				entity.Set(new TimeComp());
			}

			var system = new ForLoopSystem(world);

			Measure.Method(() => { system.Update(DeltaTime); })
				.Bench(Iterations);
		}

		[With(typeof(TimeComp))]
		public sealed class ForEachLoopSystem : AEntitySetSystem<float>
		{
			public ForEachLoopSystem(World world) : base(world)
			{
			}

			protected override void Update(float state, ReadOnlySpan<Entity> entities)
			{
				foreach (ref readonly var entity in entities)
				{
					entity.Get<TimeComp>().Value += state;
				}
			}
		}

		[Test, Performance]
		public void ForEachLoop()
		{
			var world = new World();
			for (int i = 0; i < EntityCount; i++)
			{
				var entity = world.CreateEntity();
				entity.Set(new TimeComp());
			}

			var system = new ForEachLoopSystem(world);

			Measure.Method(() => { system.Update(DeltaTime); })
				.Bench(Iterations);
		}
	}
}