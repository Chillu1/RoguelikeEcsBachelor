using DefaultEcs;
using ElRaccoone.Tweens.Core;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class PauseController
	{
		public bool IsPaused => _playerPaused || _gamePaused;

		private readonly World _world;

		private bool _playerPaused;
		private bool _gamePaused;

		public PauseController(World world)
		{
			_world = world;

			_world.Subscribe<PlayerPauseEvent>(PlayerPause);
			_world.Subscribe<PlayerResumeEvent>(PlayerResume);
			_world.Subscribe<PauseActionEvent>(GamePause);
			_world.Subscribe<ResumeActionEvent>(GameResume);
		}

		public void TogglePause()
		{
			//TODO Refactor
			if (_playerPaused)
				PlayerResume(default);
			else
				PlayerPause(default);
		}

		public void PlayerPause(in PlayerPauseEvent message)
		{
			//If game over, don't pause
			if (_world.GetEntities().With<IsPlayerComp>().AsSet().GetFirst().Get<HealthComp>().IsDead)
				return;

			_playerPaused = true;
			ITween.TweenScaleDeltaTime = 0f;
			_world.Publish(new PauseChangedEvent(IsPaused, _playerPaused));
		}

		public void PlayerResume(in PlayerResumeEvent message)
		{
			_playerPaused = false;
			ITween.TweenScaleDeltaTime = 1f;
			_world.Publish(new PauseChangedEvent(IsPaused, _playerPaused));
		}

		public void GamePause(in PauseActionEvent actionEvent)
		{
			_gamePaused = true;
			_world.Publish(new PauseChangedEvent(IsPaused, _playerPaused));
		}

		public void GameResume(in ResumeActionEvent actionEvent)
		{
			_gamePaused = false;
			_world.Publish(new PauseChangedEvent(IsPaused, _playerPaused));
		}
	}
}