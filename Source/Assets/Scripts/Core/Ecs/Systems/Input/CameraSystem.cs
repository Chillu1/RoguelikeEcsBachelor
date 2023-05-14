using System;
using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	[With(typeof(IsCameraTargetComp))]
	public sealed class CameraSystem : AEntitySetSystem<float>
	{
		private readonly Camera _camera;
		private readonly Bounds _bounds;

		public CameraSystem(World world, Camera camera, Bounds bounds) : base(world)
		{
			_camera = camera;
			_bounds = bounds;
		}

		protected override void Update(float state, in Entity entity)
		{
			ref readonly var position = ref entity.Get<PositionComp>();

			var cameraTransform = _camera.transform;
			var cameraPosition = cameraTransform.position;
			cameraPosition = Vector3.Lerp(cameraPosition, new Vector3(position.Value.x, position.Value.y, -10), 0.1f);

			float speed = (entity.Get<VelocityComp>().Speed + entity.Get<StatsComp>().ExtraMoveSpeed) *
			              entity.Get<MultiplierComp>().BaseMultipliers[(int)StatType.MoveSpeed];
			_camera.orthographicSize = Curves.CameraOrthographicSize.Evaluate(speed);

			//Keep camera inside bounds, based on camera size. If camera is bigger than bounds, clamp to bounds center.
			if (_bounds.Size.x > _camera.orthographicSize * _camera.aspect * 2 &&
			    _bounds.Size.y > _camera.orthographicSize * 2)
			{
				float offset = 75f;
				cameraPosition.x = Mathf.Clamp(cameraPosition.x, _bounds.Min.x - offset + _camera.orthographicSize * _camera.aspect,
					_bounds.Max.x + offset - _camera.orthographicSize * _camera.aspect);
				cameraPosition.y = Mathf.Clamp(cameraPosition.y, _bounds.Min.y - offset + _camera.orthographicSize,
					_bounds.Max.y + offset - _camera.orthographicSize);
			}
			else
			{
				cameraPosition.x = Mathf.Clamp(cameraPosition.x, _bounds.Center.x - 80, _bounds.Center.x + 80);
				cameraPosition.y = Mathf.Clamp(cameraPosition.y, _bounds.Center.y - 160, _bounds.Center.y + 160);
			}

			cameraTransform.position = cameraPosition;
		}
	}
}