using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class WaveSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;

		private readonly World _world;
		private readonly IReadOnlyList<WaveInfo> _waveInfos;
		private readonly GameData _gameData;

		private Wave _currentWave;

		public const int Interval = 30;
		private float _timer;
		private bool _hasTweened;

		public WaveSystem(World world, WaveData waveData, GameData gameData)
		{
			_world = world;
			_waveInfos = waveData.GetWaves();
			_gameData = gameData;

			_world.Subscribe((in FinishWaveEvent _) => FinishWave(true));

			StartNextWave();
		}

		public void Update(float state)
		{
			ref var waveInfo = ref _world.Get<WaveInfoComp>();
			waveInfo.Time = _timer;

			if (!IsEnabled)
				return;

			_currentWave.Update(state, _timer);

			_timer -= state;
			if (_timer < 0.5f && !_hasTweened)
			{
				_world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst().Set(new InvulnerableComp() { TimeLeft = 1f });
				TimeTween.Tween(0f, 1f).SetEase(EaseType.SineOut).SetOnCancel(() =>
				{
					if (_timer != Interval)
						_timer = 0;
				}).SetOnComplete(() =>
				{
					if (_timer != Interval)
						_timer = 0;
				});
				_hasTweened = true;
			}

			if (_timer > 0)
				return;

			waveInfo.WaveNumber++;
			if (_waveInfos.Count <= waveInfo.WaveNumber)
			{
				Debug.Log("Reached last wave");
				IsEnabled = false;
				return;
			}

			FinishWave(false);
		}

		public void FinishWave(bool isLoad)
		{
			ref var waveInfo = ref _world.Get<WaveInfoComp>();
			waveInfo.WaveFinished = true;
			ref readonly var player = ref _world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst();

			if (!isLoad)
			{
				_world.Publish(new CheckUnlocksEvent(_gameData, _world.Get<ScoreComp>(), player));
				_world.Publish(new AddScoreEvent());
				_world.Publish(new GetNewUpgradesEvent(false));
			}
			else
			{
				_world.Publish(new ScoreUpdatedEvent(_world.Get<GoldComp>().Value));
				_world.Publish(new GetNewUpgradesEvent(true));
			}

			_world.Publish(new WaveCompletedEvent(waveInfo.WaveNumber));
			player.Get<HealthComp>().Reset();
			_world.Get<DeadEnemiesListComp>().Value.Clear();
			_world.Publish(new PauseActionEvent());
			_world.Publish(new FrameStateEndEvent(_gameData, player));
			_hasTweened = false;

			StartNextWave();
		}

		internal void IncrementWave()
		{
			ref var waveInfo = ref _world.Get<WaveInfoComp>();
			waveInfo.WaveNumber++;
			if (_waveInfos.Count <= waveInfo.WaveNumber)
			{
				Debug.Log("Reached last wave");
				IsEnabled = false;
				return;
			}

			StartNextWave();
		}

		internal void EndWave() => _timer = 0;

		internal void SetWave(int waveIndex)
		{
			_world.Get<WaveInfoComp>().WaveNumber = waveIndex;
			StartNextWave();
		}

		private void StartNextWave()
		{
			ref var waveInfo = ref _world.Get<WaveInfoComp>();
			_timer = Interval;
			_currentWave = new Wave(_world, _waveInfos[waveInfo.WaveNumber], _world.Get<DifficultyComp>());

			Debug.Log($"Wave {waveInfo.WaveNumber + 1} started");
		}

		public void Dispose()
		{
		}
	}
}