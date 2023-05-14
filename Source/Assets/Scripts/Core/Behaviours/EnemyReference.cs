using DefaultEcs;
using UnityEngine;

namespace RoguelikeEcs.Core
{
	public class EnemyReference : MonoBehaviour
	{
		public Entity Entity;

		public void Setup(Entity entity) => Entity = entity;
	}
}