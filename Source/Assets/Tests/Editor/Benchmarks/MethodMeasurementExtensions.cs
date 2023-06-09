using Unity.PerformanceTesting.Measurements;

namespace ModifierSystemEcs.Benchmarks
{
	public static class MethodMeasurementExtensions
	{
		public static MethodMeasurement Bench(this MethodMeasurement measurement, int iterations = 5000)
		{
			measurement.WarmupCount(10)
				.MeasurementCount(50)
				.IterationsPerMeasurement(iterations)
				.Run();
			return measurement;
		}

		public static MethodMeasurement BenchGC(this MethodMeasurement measurement, int iterations = 5000)
		{
			measurement.WarmupCount(10)
				.MeasurementCount(50)
				.IterationsPerMeasurement(iterations)
				.GC()
				.Run();
			return measurement;
		}

		public static MethodMeasurement BenchGCLow(this MethodMeasurement measurement, int iterations = 5000)
		{
			measurement.WarmupCount(5)
				.MeasurementCount(20)
				.IterationsPerMeasurement(iterations)
				.GC()
				.Run();
			return measurement;
		}
	}
}