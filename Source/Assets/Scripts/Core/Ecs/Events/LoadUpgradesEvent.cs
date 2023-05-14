using System.Collections.Generic;

namespace RoguelikeEcs.Core
{
	public readonly struct LoadUpgradesEvent
	{
		public readonly Dictionary<UpgradeType, int> UpgradesData;

		public LoadUpgradesEvent(Dictionary<UpgradeType, int> upgradesData) => UpgradesData = upgradesData;
	}
}