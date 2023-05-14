using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs.System;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class CoreSystem<T> : ISystem<T>
	{
		/// <summary>
		///     Doesn't do anything for CoreSystem (always on)
		/// </summary>
		public bool IsEnabled { get; set; } = true;

		private readonly ISystem<T>[] _systems;

		private static readonly HashSet<Type> nonTestSystems = new HashSet<Type>()
		{
			typeof(MousePositionSystem),
			typeof(MouseInputSystem),
			typeof(WandRotationSystem),
			typeof(FlashEffectSystem),
			typeof(EnemyAttackedFlashSystem),
			typeof(HealthBarSystem),
			typeof(HealthDisplaySystem),
			typeof(EnemyRenderingSystem),
			typeof(PlayerDirectionRenderingSystem),
			typeof(CameraSystem),
		};

		public CoreSystem(IEnumerable<ISystem<T>> systems)
		{
			if (Core.IsTestMode)
				systems = systems.Where(s => !nonTestSystems.Contains(s.GetType()));

			_systems = systems.ToArray();
		}

		public CoreSystem(params ISystem<T>[] systems)
			: this(systems as IEnumerable<ISystem<T>>)
		{
		}

		internal TSystem GetSystem<TSystem>() where TSystem : ISystem<T>
		{
			return _systems.OfType<TSystem>().First();
		}

		public ISystem<T>[] GetSystems() => _systems;

		public void Update(T state)
		{
			foreach (var system in _systems)
				system.Update(state);
		}

		public void Dispose()
		{
			for (int i = _systems.Length - 1; i >= 0; --i)
				_systems[i].Dispose();
		}
	}
}