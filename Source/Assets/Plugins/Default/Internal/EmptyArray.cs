#if NET_STANDARD_2_0 || NETSTANDARD2_1_OR_GREATER
using System;
#endif

namespace DefaultEcs.Internal
{
	internal static class EmptyArray<T>
	{
#if NET_STANDARD_2_0 || NETSTANDARD2_1_OR_GREATER
		public static T[] Value => Array.Empty<T>();
#else
        public static T[] Value { get; } = new T[0];
#endif
	}
}