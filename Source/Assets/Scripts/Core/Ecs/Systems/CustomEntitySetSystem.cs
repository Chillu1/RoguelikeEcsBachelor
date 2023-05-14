using System;
using System.Buffers;
using System.Diagnostics;
using DefaultEcs;
using DefaultEcs.Internal.System;
using DefaultEcs.System;

namespace RoguelikeEcs.Core
{
	[Flags]
	public enum SystemParameters
	{
		None = 0,
		UsePlayer = 1 << 0,
		UseBuffer = 1 << 1,
		RunWhenEmpty = 1 << 2,
	}

	//TODO Rename
	public abstract class CustomEntitySetSystem : ISystem<float>
	{
		private readonly bool _usesPlayer, _usesInterval, _useBuffer;
		private readonly float _interval;
		private readonly EntitySet _playerEntitySet;
		private readonly bool _runWhenEmpty;

		protected float Timer;
		protected Entity Player;

		/// <summary>
		/// Gets or sets whether the current <see cref="AEntitySetSystem{T}"/> instance should update or not.
		/// </summary>
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Gets the <see cref="EntitySet"/> instance on which this system operates.
		/// </summary>
		public EntitySet Set { get; }

		/// <summary>
		/// Gets the <see cref="DefaultEcs.World"/> instance on which this system operates.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public World World { get; }

		private CustomEntitySetSystem(Func<object, EntitySet> factory)
		{
			Set = factory(this);
			World = Set.World;
		}

		/// <summary>
		/// Initialise a new instance of the <see cref="AEntitySetSystem{T}"/> class with the given <see cref="DefaultEcs.World"/> and factory.
		/// The current instance will be passed as the first parameter of the factory.
		/// </summary>
		/// <param name="world">The <see cref="DefaultEcs.World"/> from which to get the <see cref="Entity"/> instances to process the update.</param>
		/// <param name="factory">The factory used to create the <see cref="EntitySet"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="world"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="factory"/> is null.</exception>
		protected CustomEntitySetSystem(World world, Func<object, World, EntitySet> factory)
			: this(
				world is null ? throw new ArgumentNullException(nameof(world)) :
				factory is null ? throw new ArgumentNullException(nameof(factory)) : o => factory(o, world))
		{
		}

		/// <summary>
		/// Initialise a new instance of the <see cref="AEntitySetSystem{T}"/> class with the given <see cref="DefaultEcs.World"/>.
		/// To create the inner <see cref="EntitySet"/>, <see cref="WithAttribute"/> and <see cref="WithoutAttribute"/> attributes will be used.
		/// </summary>
		/// <param name="world">The <see cref="DefaultEcs.World"/> from which to get the <see cref="Entity"/> instances to process the update.</param>
		/// <param name="factory">The factory used to create the <see cref="EntitySet"/>.</param>
		/// <param name="useBuffer">Whether the entities should be copied before being processed.</param>
		/// <exception cref="ArgumentNullException"><paramref name="world"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="factory"/> is null.</exception>
		protected CustomEntitySetSystem(World world, Func<object, World, EntitySet> factory, bool useBuffer, bool runWhenEmpty)
			: this(world, factory)
		{
			_useBuffer = useBuffer;
			_runWhenEmpty = runWhenEmpty;
		}

		/// <summary>
		/// Initialise a new instance of the <see cref="AEntitySetSystem{T}"/> class with the given <see cref="DefaultEcs.World"/>.
		/// To create the inner <see cref="EntitySet"/>, <see cref="WithAttribute"/> and <see cref="WithoutAttribute"/> attributes will be used.
		/// </summary>
		/// <param name="world">The <see cref="DefaultEcs.World"/> from which to get the <see cref="Entity"/> instances to process the update.</param>
		/// <param name="useBuffer">Whether the entities should be copied before being processed.</param>
		/// <exception cref="ArgumentNullException"><paramref name="world"/> is null.</exception>
		protected CustomEntitySetSystem(World world, SystemParameters parameters = SystemParameters.None, float interval = 0) : this(world,
			DefaultFactory, parameters.HasFlag(SystemParameters.UseBuffer), parameters.HasFlag(SystemParameters.RunWhenEmpty))
		{
			_usesPlayer = parameters.HasFlag(SystemParameters.UsePlayer);
			if (parameters.HasFlag(SystemParameters.UsePlayer))
				_playerEntitySet = world.GetEntities().With<IsPlayerComp>().AsSet();
			_usesInterval = interval > 0f;
			_interval = interval;
		}

		private static EntitySet DefaultFactory(object o, World w) => EntityRuleBuilderFactory.Create(o.GetType())(o, w).AsSet();

		/// <summary>
		/// Performs a pre-update treatment.
		/// </summary>
		/// <param name="state">The state to use.</param>
		protected virtual void PreUpdate(float state)
		{
		}

		/// <summary>
		/// Performs a post-update treatment.
		/// </summary>
		/// <param name="state">The state to use.</param>
		protected virtual void PostUpdate(float state)
		{
		}

		/// <summary>
		/// Update the given <see cref="Entity"/> instance once.
		/// </summary>
		/// <param name="state">The state to use.</param>
		/// <param name="entity">The <see cref="Entity"/> instance to update.</param>
		protected virtual void Update(float state, in Entity entity)
		{
		}

		/// <summary>
		/// Update the given <see cref="Entity"/> instances once.
		/// </summary>
		/// <param name="state">The state to use.</param>
		/// <param name="entities">The <see cref="Entity"/> instances to update.</param>
		protected virtual void Update(float state, ReadOnlySpan<Entity> entities)
		{
			foreach (ref readonly Entity entity in entities)
			{
				Update(state, entity);
			}
		}

		/// <summary>
		/// Updates the system once.
		/// Does nothing if <see cref="IsEnabled"/> is false or if the inner <see cref="EntitySet"/> is empty.
		/// </summary>
		/// <param name="delta">The state to use.</param>
		public void Update(float delta)
		{
			if (!IsEnabled)
				return;

			if (_usesInterval)
			{
				Timer += delta;
				if (Timer < _interval)
					return;

				Timer = 0;
			}

			if (Set.Count > 0 || _runWhenEmpty)
			{
				if (_usesPlayer)
					Player = _playerEntitySet.GetFirst();

				PreUpdate(delta);

				if (_useBuffer)
				{
					Entity[] buffer = ArrayPool<Entity>.Shared.Rent(Set.Count);
					Set.GetEntities().CopyTo(buffer);

					Update(delta, new ReadOnlySpan<Entity>(buffer, 0, Set.Count));

					ArrayPool<Entity>.Shared.Return(buffer);
				}
				else
				{
					Update(delta, Set.GetEntities());
				}

				Set.Complete();
				if (_usesPlayer)
					_playerEntitySet.Complete();

				PostUpdate(delta);
			}
		}

		/// <summary>
		/// Disposes of the inner <see cref="EntitySet"/> instance.
		/// </summary>
		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);

			Set.Dispose();
		}
	}
}