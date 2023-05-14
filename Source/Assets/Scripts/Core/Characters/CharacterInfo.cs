using System.Linq;
using DefaultEcs;
using RoguelikeEcs.Core.Utilities;

namespace RoguelikeEcs.Core
{
	public delegate void UpgradesCompAction(ref UpgradesComp upgrades);

	public delegate void StatsCompAction(ref StatsComp stats);

	public sealed class CharacterInfo : IUnlockInfo
	{
		public string DisplayString => Name + " Char.";
		public CharacterType Type { get; }
		public bool IsUnlockable => UnlockAction != null;
		public string Name { get; private set; }
		public string Description { get; private set; }
		public string UnlockText { get; private set; }
		public bool IsEndOfWaveUnlock { get; private set; }
		public UnlockAction UnlockAction { get; private set; }

		private (StatType StatType, float Multiplier)[] _multipliers;
		private UpgradesCompAction _upgradesCompActions;
		private StatsCompAction _statsCompActions;

		public CharacterInfo(CharacterType type)
		{
			Type = type;
		}

		public CharacterInfo SetName(string name)
		{
			Name = name;
			return this;
		}

		public CharacterInfo SetDescription(string description)
		{
			Description = description;
			return this;
		}

		public CharacterInfo Unlockable(UnlockAction action, string unlockText)
		{
			UnlockAction = action;
			UnlockText = "Unlock: " + unlockText;
			return this;
		}

		public CharacterInfo UnlockableWaveEnd(UnlockAction action, string unlockText)
		{
			UnlockAction = action;
			UnlockText = "Unlock: " + unlockText;
			IsEndOfWaveUnlock = true;
			return this;
		}

		public CharacterInfo Multipliers(params (StatType StatType, float Multiplier)[] multiplier)
		{
			_multipliers = multiplier;
			return this;
		}

		public CharacterInfo Upgrade(UpgradesCompAction action)
		{
			_upgradesCompActions = action;
			return this;
		}

		public CharacterInfo Stats(StatsCompAction action)
		{
			_statsCompActions = action;
			return this;
		}

		public void Apply(ref MultiplierComp multiplier, ref UpgradesComp upgrades, ref StatsComp stats)
		{
			foreach ((var statType, float multiplierValue) in _multipliers.IsNotNull())
				multiplier.MultipliersList.Add(new MultiplierData { StatType = statType, Multiplier = multiplierValue });

			_upgradesCompActions?.Invoke(ref upgrades);
			_statsCompActions?.Invoke(ref stats);
		}

		public void OnUnlock(GameData gameData, World world, Entity player)
		{
			gameData.UnlockedCharacters.Add(Type);
			//world.Publish(new CharacterUnlockedEvent(Type));
		}
	}
}