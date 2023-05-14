using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class ScoreController
	{
		private readonly World _world;

		private readonly EntitySet _playerEntitySet;

		private const float HealthMultiplier = 10f;
		private const float KillMultiplier = 1f;
		private const float BossMultiplier = 40f;
		private const float PacifistMultiplier = 200f;
		private const float DamageTakenMultiplier = 40f;

		private const float FlatGold = 60f;
		private const float GoldMultiplier = 0.33f;

		public ScoreController(World world)
		{
			_world = world;

			_playerEntitySet = _world.GetEntities().With<IsPlayerComp>().AsSet();

			_world.Subscribe<AddScoreEvent>(AddUpScore);
		}

		private void AddUpScore(in AddScoreEvent message)
		{
			float totalScore = 0;

			ref var score = ref _world.Get<ScoreComp>();
			ref var killStreak = ref _world.Get<KillStreakComp>();
			score.SubmitStreak(killStreak.Streak);

			ref readonly var health = ref _playerEntitySet.GetFirst().Get<HealthComp>();
			totalScore += health.Percent * HealthMultiplier;
			totalScore += score.Kills * KillMultiplier;
			totalScore += score.BossesKilled * BossMultiplier;
			totalScore += Curves.DamageToScore.Evaluate(score.DamageDealt);
			totalScore += Mathf.Max(0f, 1f - (score.DamageDealt / 800f) - (score.Kills / 4f)) * PacifistMultiplier;

			totalScore += Mathf.Max(0f, 1f - score.DamageTaken / health.Max) * DamageTakenMultiplier;
			totalScore += Curves.KillStreakToScore.Evaluate(score.HighestKillStreak);

			//Debug.Log(
			//	$"Scores: From health:{health.Percent * HealthMultiplier} " +
			//	$"kills:{score.Kills * KillMultiplier} " +
			//	$"bosses:{score.BossesKilled * BossMultiplier} " +
			//	$"damage:{Curves.DamageToScore.Evaluate(score.DamageDealt)} " +
			//	$"pacifist:{Mathf.Max(0f, 1f - (score.DamageDealt / 800f) - (score.Kills / 4f)) * PacifistMultiplier} " +
			//	$"damage taken:{Mathf.Max(0f, 1f - score.DamageTaken / health.Max) * DamageTakenMultiplier} " +
			//	$"killstreak:{Curves.KillStreakToScore.Evaluate(score.HighestKillStreak)}");

			score.ResetWave(_world);
			//killStreak.Reset();

			score.TotalScore += totalScore;
			float goldChange = FlatGold + totalScore * GoldMultiplier;
			_world.Publish(new ScoreUpdatedEvent(goldChange));

			//Debug.Log("Gold earned (excluding flat):" + totalScore * GoldMultiplier);

			_world.Get<GoldComp>().Value += goldChange;

			_playerEntitySet.Complete();
		}
	}
}