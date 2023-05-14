using DefaultEcs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeEcs.Core
{
	public sealed class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private World _world;
		private MechanicType[] _mechanicTypes;
		private EnemyType _enemyType;

		private float _hoverTime;
		private bool _hovering;

		public void Setup(World world)
		{
			_world = world;
		}

		public void Set(params MechanicType[] mechanicTypes)
		{
			_mechanicTypes = mechanicTypes;
		}

		public void Set(EnemyType enemyType)
		{
			_enemyType = enemyType;
		}

		private void Update()
		{
			if (!_hovering || _hoverTime == float.MaxValue)
				return;

			_hoverTime += Time.unscaledDeltaTime;
			if (_hoverTime > 0.3f)
			{
				if (_mechanicTypes != null)
					_world.Publish(new MechanicsHoverOverEvent(_mechanicTypes));
				else if (_enemyType != EnemyType.None)
					_world.Publish(new EnemyHoverOverEvent(transform.position, _enemyType));

				_hoverTime = float.MaxValue;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if ((_mechanicTypes == null || _mechanicTypes.Length == 0) && _enemyType == EnemyType.None)
				return;

			_hoverTime = 0;
			_hovering = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_hoverTime = 0;
			_hovering = false;

			if (_mechanicTypes != null)
				_world.Publish(new MechanicsStopHoverEvent());
			else if (_enemyType != EnemyType.None)
				_world.Publish(new EnemyStopHoverEvent());
		}
	}
}