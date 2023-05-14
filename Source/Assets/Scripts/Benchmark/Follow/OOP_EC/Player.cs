using UnityEngine;

namespace RoguelikeEcs.Benchmark.OOP_EC_Follow
{
	public sealed class Player : MovingObject
	{
		private readonly HealthObject _health;

		public Player()
		{
			_health = new HealthObject
			{
				Current = 100,
				Max = 100
			};
		}

		public void Update(float deltaTime)
		{
			float time = Time.time;
			Velocity = new Vector2(
				Mathf.Sin(time) * 50,
				Mathf.Cos(time) * 50
			);

			Move(deltaTime);
		}
	}
}