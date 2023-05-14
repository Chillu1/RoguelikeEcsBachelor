using System;

namespace RoguelikeEcs.Benchmark
{
	public sealed class MovingAverage
	{
		private const int MaxSamples = 20;
		private readonly float[] _samples;

		private int _currentSampleIndex;
		private int _currentSampleCount;
		private float _currentAverage;

		public MovingAverage()
		{
			_samples = new float[MaxSamples];
			for (int i = 0; i < MaxSamples; i++)
				AddSample(0.01f);
		}

		public void AddSample(float sample)
		{
			_currentAverage -= _samples[_currentSampleIndex];
			_samples[_currentSampleIndex] = sample;
			_currentAverage += sample;

			_currentSampleIndex = (_currentSampleIndex + 1) % MaxSamples;
			_currentSampleCount = Math.Min(_currentSampleCount + 1, MaxSamples);
		}

		public float GetAverage()
		{
			return _currentAverage / _currentSampleCount;
		}

		public void Reset()
		{
			_currentSampleIndex = 0;
			_currentSampleCount = 0;
			_currentAverage = 0;
		}

		public string GetAllSamples()
		{
			string result = "";
			for (int i = 0; i < MaxSamples; i++)
				result += _samples[i] * 1000f + "ms, ";

			return result;
		}

		public override string ToString()
		{
			return (GetAverage() * 1000f).ToString("0.000") + "ms";
		}
	}
}