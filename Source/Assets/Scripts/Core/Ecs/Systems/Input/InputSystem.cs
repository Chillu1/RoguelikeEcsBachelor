using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsPlayerComp))]
	public sealed class InputSystem : AEntitySetSystem<float>
	{
		//private float _lastPressedLeftTime;
		//private float _lastPressedRightTime;

		public InputSystem(World world) : base(world)
		{
		}

		protected override void Update(float delta, in Entity entity)
		{
			float horizontalAxis = Input.GetAxis("Horizontal");
			float verticalAxis = Input.GetAxis("Vertical");

			if (horizontalAxis == 0 && verticalAxis == 0)
			{
				ref var velocityComp = ref entity.Get<VelocityComp>();
				velocityComp.Direction = Vector2.Lerp(velocityComp.Direction, Vector2.zero, 0.1f);
				return;
			}

			//Special newest keystroke priority Hack
			/*if (Input.GetKeyDown(KeyCode.A))
				_lastPressedLeftTime = Time.time;
			if (Input.GetKeyDown(KeyCode.D))
				_lastPressedRightTime = Time.time;
			if (Input.GetKeyUp(KeyCode.A))
				_lastPressedLeftTime = 0;
			if (Input.GetKeyUp(KeyCode.D))
				_lastPressedRightTime = 0;

			if (horizontalAxis != 0)
			{
				if (_lastPressedLeftTime >= _lastPressedRightTime)
					_horizontalMultiplier = -1;
				else if (_lastPressedLeftTime < _lastPressedRightTime)
					_horizontalMultiplier = 1;
				else
					_horizontalMultiplier = 1;
			}
			else
				_horizontalMultiplier = 0;*/

			var direction = new Vector2(horizontalAxis, verticalAxis).normalized;

			entity.Get<VelocityComp>().Direction = direction;
		}
	}
}