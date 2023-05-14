using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class KeyBindSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;

		private readonly World _world;
		private readonly PauseController _pauseController;

		private bool _isFullScreen;

		public KeyBindSystem(World world, PauseController pauseController)
		{
			_world = world;
			_pauseController = pauseController;
		}

		public void Update(float state)
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			if (_isFullScreen && !Screen.fullScreen) //Fullscreen hack, because website eats our escape input if in fullscreen
			{
				_pauseController.TogglePause();
				_isFullScreen = Screen.fullScreen;
				return;
			}

			_isFullScreen = Screen.fullScreen;
#endif

			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
				_pauseController.TogglePause();
		}

		public void Dispose()
		{
		}
	}
}