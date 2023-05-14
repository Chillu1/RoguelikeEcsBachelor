using System;
using System.Runtime.CompilerServices;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace ModifierSystemEcs.Benchmarks
{
	public class DefaultBenchmark
	{
		public const int Measurements = 50;
		public const int Iterations = 10;
		public const int EntityCount = 10000;

		private const float DeltaTime = 0.01666667f;

		public sealed class TestSystem
		{
			private Entity[] _entities;

			public TestSystem()
			{
				_entities = new Entity[100];
				for (int i = 0; i < _entities.Length; i++)
				{
					_entities[i] = new Entity();
				}
			}

			public void UpdateRef(float delta)
			{
				for (int i = 0; i < _entities.Length; i++)
				{
					ref readonly var entity = ref _entities[i];
					float test = entity.EntityId;
				}
			}

			public void UpdateCopy(float delta)
			{
				for (int i = 0; i < _entities.Length; i++)
				{
					var entity = _entities[i];
					float test = entity.EntityId;
				}
			}
		}

		[Test, Performance, Explicit, Category("Benchmark")]
		public void BenchRefReadonly()
		{
			var system = new TestSystem();
			float delta = 0.01666667f;
			Measure.Method(() => { system.UpdateRef(delta); })
				.Bench();
		}

		[Test, Performance, Explicit, Category("Benchmark")]
		public void BenchStructCopy()
		{
			var system = new TestSystem();
			float delta = 0.01666667f;
			Measure.Method(() => { system.UpdateCopy(delta); })
				.Bench();
		}
	}
}