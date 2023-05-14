using System;
using UnityEngine;
using Random = System.Random;

namespace RoguelikeEcs.Core
{
	/// <summary>
	///		WarCraft 3 like implementation of the pseudo random distribution.
	/// </summary>
	public sealed class PseudoRandomDistribution
	{
		private readonly int _seed;
		private readonly Random _random;
		private float _chance;

		private int _n;

		public PseudoRandomDistribution(float chance)
		{
			if (chance <= 0 || chance > 1)
				throw new ArgumentOutOfRangeException(nameof(chance));

			_seed = Environment.TickCount;
			_random = new Random(_seed);

			_chance = chance;
		}

		public bool Roll()
		{
			_n++;
			bool result = _random.NextDouble() < CFromP(_chance * _n);
			if (result)
				_n = 0;
			return result;
		}

		public static bool Roll(in float chance, ref int n)
		{
			if (chance <= 0f)
				return false;

			bool result = UnityEngine.Random.value < CFromP(chance * ++n);
			if (result)
				n = 0;
			return result;
		}

		private static float CFromP(float p)
		{
			if (p <= 0f)
				throw new ArgumentOutOfRangeException("p", "p must be greater than 0");

			float CUpper = p;
			float CLower = 0f;
			float CMid;
			float p1;
			float p2 = 1f;
			while (true)
			{
				CMid = (CUpper + CLower) / 2f;
				p1 = PfromC(CMid);
				if (Math.Abs(p1 - p2) <= 0f)
					break;

				if (p1 > p)
				{
					CUpper = CMid;
				}
				else
				{
					CLower = CMid;
				}

				p2 = p1;
			}

			return CMid;
		}

		private static float PfromC(float C)
		{
			float pProcOnN = 0f;
			float pProcByN = 0f;
			float sumNpProcOnN = 0f;

			int maxFails = (int)Math.Ceiling(1f / C);
			for (int n = 1; n <= maxFails; ++n)
			{
				pProcOnN = Math.Min(1f, n * C) * (1f - pProcByN);
				pProcByN += pProcOnN;
				sumNpProcOnN += n * pProcOnN;
			}

			return 1f / sumNpProcOnN;
		}
	}
}