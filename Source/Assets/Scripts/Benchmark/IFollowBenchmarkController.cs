using UnityEngine;

namespace RoguelikeEcs.Benchmark
{
	public interface IFollowBenchmarkController : IBenchmarkController
	{
		const float EnemySpeed = 30f;
		static readonly Vector2 PlayerSpawnPosition = new Vector2(50, 50);
		
		Vector2 GetPlayerPosition();
		Vector2 GetEnemyPosition();
		Vector2 GetEnemyVelocity();
	}
}