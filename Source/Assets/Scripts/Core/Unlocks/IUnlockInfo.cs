using System;
using System.Diagnostics.CodeAnalysis;
using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public delegate bool UnlockAction(GameData gameData, World world, Entity player, ScoreComp score);

	public interface IUnlockInfo
	{
		bool IsEndOfWaveUnlock { get; }
		string DisplayString { get; }
		UnlockAction UnlockAction { get; }

		void OnUnlock(GameData gameData, World world, Entity player);
	}

	public static class UnlockInfoExtensions
	{
		[SuppressMessage("ReSharper", "AccessToModifiedClosure")]
		public static void Subscribe(this IUnlockInfo info, World world)
		{
			if (!info.IsEndOfWaveUnlock)
			{
				IDisposable frameEndSubscription = null;
				frameEndSubscription = world.Subscribe((in FrameStateEndEvent message) =>
				{
					if (!info.UnlockAction(message.GameData, world, message.Player, world.Get<ScoreComp>()))
						return;

					Debug.Log($"Unlocked: {info.DisplayString}");
					world.Publish(new UnlockEvent(info));
					info.OnUnlock(message.GameData, world, message.Player);

					frameEndSubscription!.Dispose();
				});
			}
			else
			{
				IDisposable roundEndSubscription = null;
				roundEndSubscription = world.Subscribe((in CheckUnlocksEvent message) =>
				{
					if (!info.UnlockAction(message.GameData, world, message.Player, message.Score))
						return;

					Debug.Log($"Unlocked: {info.DisplayString}");
					world.Publish(new UnlockEvent(info));
					info.OnUnlock(message.GameData, world, message.Player);

					roundEndSubscription!.Dispose();
				});
			}
		}
	}
}