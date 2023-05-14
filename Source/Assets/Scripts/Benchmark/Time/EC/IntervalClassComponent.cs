namespace RoguelikeEcs.Benchmark.EC_Time
{
	internal class IntervalClassComponent
	{
		private float _interval;
		private float _time;

		private int _counter;

		public void Update(float deltaTime)
		{
			_time += deltaTime;

			if (_time >= _interval)
			{
				_time = 0;
				_counter++;
			}
		}
	}
}