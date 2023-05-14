namespace RoguelikeEcs.Core
{
	public struct HealthRegenComp
	{
		public float Value;
		public const float Interval = 0.1f;

		public float Timer;
	}
}