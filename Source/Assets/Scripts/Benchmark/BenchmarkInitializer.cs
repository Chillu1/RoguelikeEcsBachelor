using System;
using UnityEngine;

namespace RoguelikeEcs.Benchmark
{
	public sealed class BenchmarkInitializer : MonoBehaviour
	{
		public static int EntityAddCount = 1000;
		public const float MaxFrameTime = 0.016666667f;

		[SerializeField]
		private BenchmarkType _benchmarkType;

		[SerializeField]
		private bool _isDebug;

		private MovingAverage _averageIterationTime;
		private IBenchmarkController _benchmarkController;

		private void Start()
		{
			_averageIterationTime = new MovingAverage();

			SetBenchmarkController();

			if (_isDebug)
				EntityAddCount = 1;
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;

			_benchmarkController.Update(deltaTime);
			if (_isDebug && _benchmarkController is IFollowBenchmarkController followBenchmarkController)
			{
				var playerPosition = followBenchmarkController.GetPlayerPosition();
				Debug.DrawLine(playerPosition + new Vector2(-5, -5), playerPosition + new Vector2(5, -5), Color.green);
				Debug.DrawLine(playerPosition + new Vector2(5, -5), playerPosition + new Vector2(5, 5), Color.green);
				Debug.DrawLine(playerPosition + new Vector2(5, 5), playerPosition + new Vector2(-5, 5), Color.green);
				Debug.DrawLine(playerPosition + new Vector2(-5, 5), playerPosition + new Vector2(-5, -5), Color.green);

				var enemyPosition = followBenchmarkController.GetEnemyPosition();
				var enemyVelocity = followBenchmarkController.GetEnemyVelocity();
				Debug.DrawLine(enemyPosition, enemyPosition + enemyVelocity, Color.red);
			}

			if (_averageIterationTime.GetAverage() > MaxFrameTime)
			{
				Debug.Log("Benchmark finished: " + _benchmarkType + " with frame time: " + _averageIterationTime + " and " +
				          _benchmarkController.GetEntityCount().ToString("N0") + " entities");
				Debug.Log("Last samples: " + _averageIterationTime.GetAllSamples());

				UnityEditor.EditorApplication.isPlaying = false;
			}
		}

		private void SetBenchmarkController()
		{
			switch (_benchmarkType)
			{
				case BenchmarkType.ECS_Follow:
					_benchmarkController = new ECS_Follow.BenchmarkController(_averageIterationTime);
					break;
				case BenchmarkType.UNITY_EC_Follow:
					if (!_isDebug)
						EntityAddCount = 100;
					_benchmarkController = new UNITY_EC_Follow.CoreSystem(_averageIterationTime);
					break;
				case BenchmarkType.EC_Follow:
					_benchmarkController = new EC_Follow.CoreSystem(_averageIterationTime);
					break;
				case BenchmarkType.OOP_EC_Follow:
					_benchmarkController = new OOP_EC_Follow.CoreSystem(_averageIterationTime);
					break;

				case BenchmarkType.ECS_Time:
					if (!_isDebug)
						EntityAddCount = 100000;
					_benchmarkController = new ECS_Time.CoreSystem(_averageIterationTime);
					break;
				case BenchmarkType.EC_Time:
					if (!_isDebug)
						EntityAddCount = 100000;
					_benchmarkController = new EC_Time.CoreSystem(_averageIterationTime);
					break;
				case BenchmarkType.UNITY_EC_Time:
					if (!_isDebug)
						EntityAddCount = 100;
					_benchmarkController = new UNITY_EC_Time.CoreSystem(_averageIterationTime);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}