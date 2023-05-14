using ElRaccoone.Tweens;
using ElRaccoone.Tweens.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeEcs.Core.Utilities
{
	public static class Tweens
	{
		public static void TweenError(this Component component)
		{
			if (component.ContainsTween())
				return;

			var originalPos = component.transform.localPosition;
			var originalColor = component.GetComponent<Graphic>().color;
			component
				.TweenGraphicColor(Color.red, 0.05f * 4)
				.TweenLocalPositionX(originalPos.x - 3, 0.05f)
				.TweenLocalPositionX(originalPos.x + 3, 0.05f).SetPingPong().SetLoopCount(2).SetDelay(0.05f)
				.TweenGraphicColor(originalColor, 0.05f).SetDelay(0.05f * 5)
				.TweenLocalPositionX(originalPos.x, 0.05f).SetDelay(0.05f * 5);
		}

		public static void TweenFlashColor(this Component component, Color color, float duration = 0.2f)
		{
			if (component.ContainsTween())
				return;

			component.TweenGraphicColor(color, duration)
				.TweenGraphicColor(Color.white, duration / 4f).SetDelay(duration);
		}

		public static void TweenFlashScale(this Component component, float scale, float duration = 0.2f)
		{
			component.TweenLocalScale(Vector3.one * scale, duration)
				.TweenLocalScale(Vector3.one, duration / 4f).SetDelay(duration);
		}

		private static readonly Color enabledColor = new Color(0.4f, 1f, 0.4f);

		public static void TweenToggleEnabled(this Component component, bool enabled)
		{
			if (component.TryGetTween(out var tween))
				tween.Cancel(); //TODO We could cancel other tweens as well, but we'd need to keep track of original position and go to it 

			component.TweenGraphicColor(enabled ? enabledColor : Color.white, 0.8f);
		}

		public static void TweenChanged(this Component component, bool changed)
		{
			if (component.TryGetTween(out var tween))
				tween.Cancel(); //TODO We could cancel other tweens as well, but we'd need to keep track of original position and go to it 

			float scale = changed ? 1.2f : 0.8f;

			component.TweenLocalScale(Vector3.one * scale, 0.3f).SetEase(EaseType.CubicIn)
				.TweenGraphicColor(changed ? Color.gray : Color.white, 0.3f)
				.TweenLocalScale(Vector3.one, 0.3f).SetEase(EaseType.CubicOut).SetDelay(0.3f)
				.TweenGraphicColor(Color.white, 0.3f).SetDelay(0.3f);
		}

		public static Tween<float> TweenAppearCanvasGroup(this GameObject gameObject)
		{
			if (gameObject.TryGetTween(out var tween))
				tween.Cancel();

			return gameObject.TweenCanvasGroupAlpha(1f, 0.3f).SetEase(EaseType.CubicOut);
		}

		public static Tween<float> TweenAppearCanvasGroup(this Component component)
		{
			if (component.TryGetTween(out var tween))
				tween.Cancel();

			return component.TweenCanvasGroupAlpha(1f, 0.3f).SetEase(EaseType.CubicOut);
		}

		public static Tween<float> TweenAppearCanvasGroupCancel(this GameObject gameObject)
		{
			if (gameObject.TryGetTween(out var tween))
				tween.Cancel();

			return gameObject.TweenCanvasGroupAlpha(1f, 0.3f).SetEase(EaseType.CubicOut).SetFrom(0f)
				.SetOnCancel(() => gameObject.GetComponent<CanvasGroup>().alpha = 1f);
		}

		public static Tween<float> TweenDisappearCanvasGroup(this GameObject gameObject)
		{
			if (gameObject.TryGetTween(out var tween))
				tween.Cancel();

			return gameObject.TweenCanvasGroupAlpha(0f, 0.3f).SetEase(EaseType.CubicIn);
		}

		public static Tween<float> TweenDisappearCanvasGroup(this Component component)
		{
			if (component.TryGetTween(out var tween))
				tween.Cancel();

			return component.TweenCanvasGroupAlpha(0f, 0.3f).SetEase(EaseType.CubicIn);
		}
	}
}