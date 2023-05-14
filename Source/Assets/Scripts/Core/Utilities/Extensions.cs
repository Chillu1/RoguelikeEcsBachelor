using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DefaultEcs;
using ElRaccoone.Tweens.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace RoguelikeEcs.Core.Utilities
{
	public static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetMagnitude(this Vector2 v)
		{
			return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
		}

		public static void SetSingletonInstance<T>(this World world, T instance = default) where T : struct
		{
			world.SetMaxCapacity<T>(1);
			world.Set(instance);
		}

		public static ICollection<T> IsNotNull<T>(this ICollection<T> collection)
		{
			return collection == null ? Array.Empty<T>() : collection;
		}

		public static bool ContainsTween(this GameObject gameObject)
		{
			return gameObject.GetComponent<ITween>() != null;
		}

		public static bool ContainsTween(this Component component)
		{
			return component.GetComponent<ITween>() != null;
		}

		public static bool TryGetTween(this GameObject gameObject, out ITween tween)
		{
			return gameObject.TryGetComponent(out tween);
		}

		public static bool TryGetTween(this Component component, out ITween tween)
		{
			return component.TryGetComponent(out tween);
		}

		//Newtonsoft json doesn't support generics, and using object might result in boxing & loss of performance
		public static void WriteValue(this JsonTextWriter writer, string propertyName, string writeObject)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(writeObject);
		}

		public static void WriteValue(this JsonTextWriter writer, string propertyName, float writeObject)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(writeObject);
		}

		public static void WriteValue(this JsonTextWriter writer, string propertyName, DateTime writeObject)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(writeObject);
		}

		public static void WriteValue(this JsonTextWriter writer, string propertyName, bool writeObject)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(writeObject);
		}

		public static Color Alpha(this Color color, float alpha)
		{
			color.a = alpha;
			return color;
		}
	}
}