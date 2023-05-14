namespace RoguelikeEcs.Core
{
	public interface IShallowClone<out T>
	{
		T ShallowClone();
	}
}