using RoguelikeEcs.Core.Utilities;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public sealed class DataInitializer : Singleton
	{
		public DataController DataController { get; private set; }

		public void Start()
		{
			if (Instance == null || (GetInstanceID() == Instance.GetInstanceID() && ((DataInitializer)Instance).DataController == null))
			{
#if UNITY_WEBGL
				Application.targetFrameRate = 60;
#else
				Application.targetFrameRate = 144;
#endif
				DataController = new DataController();
			}
		}
	}
}