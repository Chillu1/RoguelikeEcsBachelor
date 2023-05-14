using System;
using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public delegate void UpgradeAction(float upgradeMultiplier, ref StatsComp stats, ref UpgradesComp playerUpgrades,
		ref MultiplierComp multiplier, ref EnemyUpgradesComp enemyUpgrades);

	public sealed class Upgrade : IUnlockInfo, IShallowClone<Upgrade>, IComparable<Upgrade>
	{
		public UpgradeType Type { get; }
		public string Name { get; private set; }
		public string DisplayString => Type + " Upg.";
		public string Description { get; private set; }
		public int Weight { get; private set; }
		public bool IsUnlockable => UnlockAction != null;
		public bool IsEndOfWaveUnlock { get; }
		public UnlockAction UnlockAction { get; private set; }
		public UpgradeRarity Rarity { get; private set; }
		public bool IsReappliable { get; private set; }
		public MechanicType[] Mechanics { get; private set; }

		private bool _isDiminishing;
		private float _diminishingValue;
		private UpgradeAction _upgradeAction;

		private bool _applied;
		private float _upgradeMultiplier;

		public Upgrade(UpgradeType type)
		{
			Type = type;
			Rarity = UpgradeRarity.Common;
			_upgradeMultiplier = 1f;
		}

		private Upgrade(Upgrade upgrade)
		{
			Type = upgrade.Type;
			Name = upgrade.Name;
			Description = upgrade.Description;
			Weight = upgrade.Weight;
			Rarity = upgrade.Rarity;
			IsReappliable = upgrade.IsReappliable;
			_isDiminishing = upgrade._isDiminishing;
			_diminishingValue = upgrade._diminishingValue;
			Mechanics = upgrade.Mechanics;
			_upgradeAction = upgrade._upgradeAction;
			UnlockAction = upgrade.UnlockAction;

			_applied = upgrade._applied;
			_upgradeMultiplier = upgrade._upgradeMultiplier;
		}

		public Upgrade SetName(string name)
		{
			Name = name;
			return this;
		}

		public Upgrade SetDescription(string description)
		{
			Description = description;
			return this;
		}

		public Upgrade SetWeight(int weight)
		{
			Weight = weight;
			return this;
		}

		public Upgrade SetUnlockable(UnlockAction action)
		{
			UnlockAction = action;
			return this;
		}

		public Upgrade SetRarity(UpgradeRarity rarity)
		{
			Rarity = rarity;
			return this;
		}

		public Upgrade SetIsReappliable(bool isReappliable = true)
		{
			IsReappliable = isReappliable;
			return this;
		}

		public Upgrade SetIsDiminishing(float diminishingValue = 0.05f)
		{
			_isDiminishing = true;
			_diminishingValue = diminishingValue;
			return this;
		}

		public Upgrade SetMechanics(params MechanicType[] mechanics)
		{
			Mechanics = mechanics;
			return this;
		}

		public Upgrade SetAction(UpgradeAction action)
		{
			_upgradeAction = action;
			return this;
		}

		public void Validate()
		{
			if (string.IsNullOrEmpty(Name))
				Debug.LogError($"Upgrade {Type} has no name");
			if (Weight <= 0)
				Debug.LogError($"Upgrade {Type} has invalid weight");
			if (_upgradeAction == null)
				Debug.LogError($"Upgrade {Type} has no action");
			if (!IsReappliable && _isDiminishing)
				Debug.LogError($"Upgrade {Type} is not reappliable, but is diminishing");
		}

		public void OnUnlock(GameData gameData, World world, Entity player)
		{
			gameData.UnlockedUpgrades.Add(Type);
			world.Publish(new UpgradeUnlockedEvent(Type));
		}

		public void Apply(ref StatsComp stats, ref UpgradesComp playerUpgrades, ref MultiplierComp multiplier,
			ref EnemyUpgradesComp enemyUpgrades)
		{
			if (!IsReappliable && _applied)
			{
				Debug.LogWarning($"Upgrade {Name} already applied. While it's not reappliable.");
				return;
			}

			_upgradeAction(_upgradeMultiplier, ref stats, ref playerUpgrades, ref multiplier, ref enemyUpgrades);

			if (_isDiminishing)
				_upgradeMultiplier *= 1f - _diminishingValue;

			_applied = true;
		}

		public override string ToString()
		{
			return $"{Type}. {Name}: {Description}";
		}

		public Upgrade ShallowClone() => new Upgrade(this);

		public int CompareTo(Upgrade other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (ReferenceEquals(null, other)) return 1;
			return Type.CompareTo(other.Type);
		}
	}
}