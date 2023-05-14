using System.Collections.Generic;
using DefaultEcs;

namespace RoguelikeEcs.Core
{
	public struct PiercingComp
	{
		public int HitsLeft;
		public HashSet<Entity> EntitiesHit;
	}
}