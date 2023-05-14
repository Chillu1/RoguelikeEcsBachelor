using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public class MousePositionSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;
		private readonly World _world;
		private readonly Camera _camera;

		public MousePositionSystem(World world, Camera camera)
		{
			world.Set<MousePositionComp>();
			_world = world;
			_camera = camera;
		}

		public void Update(float delta)
		{
			Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
			_world.Get<MousePositionComp>().Value = worldPoint;
		}

		public void Dispose()
		{
		}
	}
}