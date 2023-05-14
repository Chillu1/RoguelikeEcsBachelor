using UnityEngine;

namespace RoguelikeEcs.Benchmark.UNITY_EC_Follow
{
	public class EnemyFollow : MonoBehaviour
	{
		public Vector3 Velocity { get; private set; }

		private Transform _playerTransform;

		private void Start()
		{
			_playerTransform = GameObject.FindWithTag("Player").transform;
		}

		private void Update()
		{
			Velocity = (_playerTransform.position - transform.position).GetNormalized() * IFollowBenchmarkController.EnemySpeed;
			transform.position += Velocity * Time.deltaTime;
		}
	}
}