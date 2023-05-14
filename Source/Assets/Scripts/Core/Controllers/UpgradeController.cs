using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using UnityEngine;
using Weighted_Randomizer;

namespace RoguelikeEcs.Core
{
	public sealed class UpgradeController
	{
		private readonly World _world;
		private readonly IReadOnlyDictionary<UpgradeType, Upgrade> _upgrades;
		private readonly DynamicWeightedRandomizer<Upgrade> _upgradesRandomizer;
		private readonly EntitySet _playerEntitySet;

		public UpgradeController(World world, UpgradesData upgradesData)
		{
			_world = world;

			_upgradesRandomizer = new DynamicWeightedRandomizer<Upgrade>();
			_upgrades = upgradesData.GetUpgrades();
			foreach (var upgrade in _upgrades.Values)
			{
				if (!upgrade.IsUnlockable)
					_upgradesRandomizer.Add(upgrade, upgrade.Weight);
			}

			_playerEntitySet = _world.GetEntities().With<IsPlayerComp>().AsSet();

			world.Subscribe((in GetNewUpgradesEvent message) => NewUpgrades(false, message.IsLoad));
			world.Subscribe((in ShopRefreshEvent _) => NewUpgrades(true));
			world.Subscribe((in UpgradeChosenEvent message) => ApplyAndSaveUpgrade(message.UpgradeType));
			world.Subscribe<LoadUpgradesEvent>(OnLoadUpgrades);
			world.Subscribe<UpgradeUnlockedEvent>(AddUpgradeToRandomizer);
		}

		private void NewUpgrades(bool isRefresh, bool isLoad = false)
		{
			Upgrade[] upgrades;
			if (!isLoad)
			{
				upgrades = new Upgrade[3];
				for (int i = 0; i < upgrades.Length; i++)
					upgrades[i] = _upgradesRandomizer.NextWithRemoval();
				foreach (var upgrade in upgrades)
					_upgradesRandomizer.Add(upgrade, upgrade.Weight);

				_world.Get<ShopUpgradesComp>().IsBought = new bool[3];
			}
			else
			{
				upgrades = _world.Get<ShopUpgradesComp>().Upgrades.Select(type => _upgrades[type]).ToArray();
			}

			_world.Get<ShopUpgradesComp>().Upgrades = upgrades.Select(u => u.Type).ToArray();
			_world.Publish(new NewUpgradesEvent(isRefresh, upgrades));
		}

		internal void ApplyAndSaveUpgrade(UpgradeType upgradeType)
		{
			ApplyUpgrade(upgradeType);

			ref var playerUpgrades = ref _world.Get<UpgradesComp>();
			if (playerUpgrades.UpgradesData.ContainsKey(upgradeType))
				playerUpgrades.UpgradesData[upgradeType]++;
			else
				playerUpgrades.UpgradesData.Add(upgradeType, 1);
		}

		private void ApplyUpgrade(UpgradeType upgradeType)
		{
			var upgrade = _upgrades[upgradeType];
			if (!upgrade.IsReappliable)
				_upgradesRandomizer.Remove(upgrade);

			ref readonly var player = ref _playerEntitySet.GetFirst();
			ref var playerUpgrades = ref _world.Get<UpgradesComp>();
			upgrade.Apply(ref player.Get<StatsComp>(), ref playerUpgrades, ref player.Get<MultiplierComp>(),
				ref _world.Get<EnemyUpgradesComp>());

			_playerEntitySet.Complete();
		}

		private void OnLoadUpgrades(in LoadUpgradesEvent loadUpgradesEvent)
		{
			foreach (var upgradeData in loadUpgradesEvent.UpgradesData)
			{
				for (int i = 0; i < upgradeData.Value; i++)
					ApplyUpgrade(upgradeData.Key);
			}
		}

		private void AddUpgradeToRandomizer(in UpgradeUnlockedEvent message)
		{
			if (_upgradesRandomizer.Contains(_upgrades[message.Type]))
			{
				Debug.LogError($"Upgrade {message.Type} is already added to randomizer");
				return;
			}

			var upgrade = _upgrades[message.Type];
			_upgradesRandomizer.Add(upgrade, upgrade.Weight);
		}
	}
}