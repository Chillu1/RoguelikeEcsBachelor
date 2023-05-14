using System;
using System.Runtime.CompilerServices;
using DefaultEcs;
using DefaultEcs.Internal;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModifierSystemEcs.Benchmarks
{
	public class FollowBenchmark
	{
		public const int Iterations = 1;
		public const int EntityCount = 340_000;

		private const float DeltaTime = 0.01666667f;

		private struct PositionComp
		{
			public Vector2 Value;
		}

		private struct VelocityComp
		{
			public Vector2 Value;
		}

		private struct IsPlayerComp
		{
		}

		private struct IsFollowerComp
		{
		}

		[Test, Performance, Explicit, Category("Benchmark")]
		public void BenchFollow()
		{
			var world = new World();
			var player = world.CreateEntity();

			player.Set<IsPlayerComp>();
			player.Set(new PositionComp { Value = new Vector2(50, 50) });
			player.Set<VelocityComp>();

			var followers = new Entity[EntityCount];
			for (int i = 0; i < EntityCount; i++)
			{
				followers[i] = world.CreateEntity();
				followers[i].Set<IsFollowerComp>();
				followers[i].Set(new PositionComp { Value = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)) });
				followers[i].Set<VelocityComp>();
			}

			var positionPool = ComponentManager<PositionComp>.GetOrCreate(world.WorldId);
			var velocityPool = ComponentManager<VelocityComp>.GetOrCreate(world.WorldId);

			var playerPosition = player.Get<PositionComp>().Value;

			Measure.Method(() =>
				{
					for (int i = 0; i < followers.Length; i++)
					{
						ref readonly var entity = ref followers[i];
						ref var position = ref positionPool.Get(entity.EntityId);
						ref var velocity = ref velocityPool.Get(entity.EntityId);
						velocity.Value = (playerPosition - position.Value).GetNormalized() * 30f;
						position.Value = velocity.Value * DeltaTime;
					}
				})
				.SetUp(() =>
				{
					for (int i = 0; i < followers.Length; i++)
						followers[i].Set(new PositionComp { Value = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)) });
				})
				.Bench(Iterations);
		}
	}

	internal static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetMagnitude(this Vector2 v)
		{
			return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 GetNormalized(this Vector2 v)
		{
			float magnitude = v.GetMagnitude();
			return magnitude == 0 ? Vector2.zero : new Vector2(v.x / magnitude, v.y / magnitude);
		}
	}
}