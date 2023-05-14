namespace RoguelikeEcs.Core
{
	/// <summary>
	///		Avoids all damage for <see cref="TimeLeft"/> seconds.
	/// </summary>
	public struct InvulnerableComp
	{
		public float TimeLeft;
	}
}