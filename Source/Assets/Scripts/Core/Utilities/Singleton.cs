using UnityEngine;

namespace RoguelikeEcs.Core.Utilities
{
	public abstract class Singleton : MonoBehaviour
	{
		protected static Singleton Instance;

		protected virtual void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
				return;
			}

			if (GetInstanceID() != Instance.GetInstanceID())
				Destroy(gameObject);
		}

		protected virtual void OnDestroy()
		{
			if (GetInstanceID() == Instance.GetInstanceID())
				Instance = null;
		}
	}
}