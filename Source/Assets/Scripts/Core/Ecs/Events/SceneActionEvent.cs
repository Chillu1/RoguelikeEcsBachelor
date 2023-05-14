namespace RoguelikeEcs.Core
{
	public readonly struct SceneActionEvent
	{
		public readonly SceneType Scene;

		public SceneActionEvent(SceneType scene) => Scene = scene;
	}
}