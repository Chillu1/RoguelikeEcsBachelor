using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class Wave
	{
		private readonly World _world;
		private readonly WaveInfo _waveInfo;
		private readonly float _difficultyMultiplier, _enrageMultiplier;

		private float _timer;
		private float _intervalTimer;
		private int _currentScore;
		private int _currentIntervalScore;

		private bool _reachedMaxScore;

		public Wave(World world, WaveInfo waveInfo, DifficultyComp difficulty)
		{
			_world = world;
			_waveInfo = waveInfo;
			_difficultyMultiplier = difficulty.Value.GetSpawnIntervalMultiplier();
			_enrageMultiplier = difficulty.EnragedWave ? 0.7f : 1f;
		}

		public void Update(float state, float timeLeft)
		{
			if (_currentScore >= _waveInfo.MaxEnemyScore)
			{
				if (_reachedMaxScore == false)
				{
					Debug.Log("Reached max score at " + timeLeft);
					_reachedMaxScore = true;
				}

				return;
			}

			_timer += state;
			_intervalTimer += state;

			if (_timer < _waveInfo.SpawnInterval * _difficultyMultiplier * _enrageMultiplier)
				return;

			_timer = 0;
			if (_intervalTimer > 5f)
			{
				_currentIntervalScore = 0;
				_intervalTimer = 0;
			}

			if (_currentIntervalScore >= _waveInfo.MaxEnemyScoreInterval)
				return;

			var enemyType = _waveInfo.GetRandomEnemy(timeLeft / WaveSystem.Interval);
			int score = EnemyData.GetScore(enemyType);
			_currentScore += (int)(score * _difficultyMultiplier * _enrageMultiplier);
			//Debug.Log(GetScoreText());
			_currentIntervalScore += (int)(score * _difficultyMultiplier * _enrageMultiplier);

			_world.Publish(new SpawnEnemyEvent(enemyType));
		}

		public string GetScoreText() => $"{_currentScore}/{_waveInfo.MaxEnemyScore}";
	}
}