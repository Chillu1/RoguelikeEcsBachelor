using UnityEngine;

namespace RoguelikeEcs.Benchmark.UNITY_EC_Time
{
	public class IntervalComponent : MonoBehaviour
	{
		private float _interval = 1f;
		private float _time;

		private int _counter;

		private void Update()
		{
			_time += Time.deltaTime;
			if (_time >= _interval)
			{
				_time = 0;
				_counter++;
			}
		}
	}
}