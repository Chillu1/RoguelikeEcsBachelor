using System.Runtime.CompilerServices;

namespace RoguelikeEcs.Core
{
	public struct DestroyComp
	{
		/// <summary>
		///		When this component is added to an entity, it will be destroyed after this many frames
		/// </summary>
		/// <default>0</default>
		public int Counter;

		public bool Internal;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Increment()
		{
			if (Counter < 1)
				Counter++;
		}
	}
}