using System;
using DefaultEcs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoguelikeEcs.Core
{
	public enum SceneType
	{
		None,
		MainMenu,
		Game,
	}

	public sealed class SceneController
	{
		private readonly World _world;

		public SceneController(World world)
		{
			_world = world;

			_world.Subscribe<SceneActionEvent>(OnSceneAction);
			_world.Subscribe<QuitActionEvent>(OnQuitAction);
		}

		private void OnSceneAction(in SceneActionEvent message)
		{
			_world.Publish(new PlayerResumeEvent());
			_world.Publish(new ResumeActionEvent());

			switch (message.Scene)
			{
				case SceneType.MainMenu:
					SceneManager.LoadScene("MainMenu");
					break;
				case SceneType.Game:
					SceneManager.LoadScene("GameplayScene");
					break;
				default:
					Debug.LogError($"Unknown scene type: {message.Scene}");
					break;
			}

			SceneManager.sceneLoaded += OnSceneLoaded;

			void OnSceneLoaded(Scene scene, LoadSceneMode mode)
			{
				_world.Dispose();
				SceneManager.sceneLoaded -= OnSceneLoaded;
			}
		}

		private void OnQuitAction(in QuitActionEvent message)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}