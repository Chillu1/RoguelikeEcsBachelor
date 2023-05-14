using UnityEngine;

namespace RoguelikeEcs.Benchmark.UNITY_EC_Follow
{
	public class PlayerMovement : MonoBehaviour
	{
		private void FixedUpdate()
		{
			float time = Time.time;
			transform.position = new Vector3(Mathf.Sin(time) * 50, Mathf.Cos(time) * 50);
		}
	}
}