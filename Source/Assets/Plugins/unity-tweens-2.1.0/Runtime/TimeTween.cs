using ElRaccoone.Tweens.Core;
using UnityEngine;

namespace ElRaccoone.Tweens
{
	public static class TimeTween
	{
		private static GameObject _instance;

		public static Tween<float> Tween(float to, float duration)
		{
			if (_instance == null)
				_instance = new GameObject("TimeTween");

			return Tween<float>.Add<Driver>(_instance).Finalize(to, duration);
		}

		private class Driver : Tween<float>
		{
			public override bool OnInitialize() => true;

			public override float OnGetFrom()
			{
				return Time.timeScale;
			}

			public override void OnUpdate(float easedTime)
			{
				this.valueCurrent = this.InterpolateValue(this.valueFrom, this.valueTo, easedTime);
				Time.timeScale = this.valueCurrent;
			}
		}
	}
}