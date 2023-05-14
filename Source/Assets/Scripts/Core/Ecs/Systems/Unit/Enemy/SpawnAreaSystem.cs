using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class SpawnAreaSystem : ISystem<float>
	{
		public bool IsEnabled { get; set; } = true;

		private readonly World _world;
		private readonly Camera _camera;
		private readonly Bounds _bounds;

		public SpawnAreaSystem(World world, Camera camera, Bounds bounds)
		{
			_world = world;
			_camera = camera;
			_bounds = bounds;
		}

		public void Update(float state)
		{
			ref var spawnArea = ref _world.Get<SpawnAreaComp>();

			foreach (var area in spawnArea.Value.Areas)
			{
				Debug.DrawLine(area.Min, area.Max, Color.red);
				Debug.DrawLine(new Vector2(area.Min.x, area.Max.y), new Vector2(area.Max.x, area.Min.y), Color.red);
			}

			spawnArea.UpdateTimer += state;
			if (spawnArea.UpdateTimer < 0.1f)
				return;

			spawnArea.UpdateTimer = 0;

			var cameraViewPortTop = _camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
			var cameraViewPortRight = _camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
			var cameraViewPortLeft = _camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
			var cameraViewPortBot = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f));

			//Create SpawnAreaOne, from camera view port TopLeft to bounds
			Vector2 minRight = new Vector2(cameraViewPortRight.x, _bounds.Min.y);
			Vector2 maxRight = new Vector2(_bounds.Max.x, _bounds.Max.y);
			spawnArea.Value.SetSpawnArea(0, minRight, maxRight);

			Vector2 minLeft = new Vector2(_bounds.Min.x, _bounds.Min.y);
			Vector2 maxLeft = new Vector2(cameraViewPortLeft.x, _bounds.Max.y);
			spawnArea.Value.SetSpawnArea(1, minLeft, maxLeft);

			Vector2 minTop = new Vector2(_bounds.Min.x, cameraViewPortTop.y);
			Vector2 maxTop = new Vector2(_bounds.Max.x, _bounds.Max.y);
			spawnArea.Value.SetSpawnArea(2, minTop, maxTop);

			Vector2 minBot = new Vector2(_bounds.Min.x, _bounds.Min.y);
			Vector2 maxBot = new Vector2(_bounds.Max.x, cameraViewPortBot.y);
			spawnArea.Value.SetSpawnArea(3, minBot, maxBot);
		}

		public void Dispose()
		{
		}
	}
}