using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeEcs.Core
{
	public sealed class HoverOverMainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private Action<Vector3, CharacterInfo> _onHover;
		private Action _onStopHover;
		private CharacterInfo _characterInfo;

		private float _hoverTime;
		private bool _hovering;

		public void Setup(Action<Vector3, CharacterInfo> onHover, Action onStopHover, CharacterInfo characterInfo)
		{
			_onHover = onHover;
			_onStopHover = onStopHover;
			_characterInfo = characterInfo;
		}

		private void Update()
		{
			if (!_hovering || _hoverTime == float.MaxValue)
				return;

			_hoverTime += Time.unscaledDeltaTime;
			if (_hoverTime <= 0.3f)
				return;

			_onHover(transform.position, _characterInfo);

			_hoverTime = float.MaxValue;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_hoverTime = 0;
			_hovering = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_hoverTime = 0;
			_hovering = false;

			_onStopHover();
		}
	}
}