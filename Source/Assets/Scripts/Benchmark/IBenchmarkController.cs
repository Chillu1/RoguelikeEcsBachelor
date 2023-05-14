namespace RoguelikeEcs.Benchmark
{
	public interface IBenchmarkController
	{
		int GetEntityCount();

		void Update(float deltaTime);
	}
}