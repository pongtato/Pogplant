﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting.Utilities
{
	public struct Ease
	{
		public static float EaseOutElastic(float x, float magnitude)
		{
			const float c4 = (2.0f * (float)Math.PI) / 3.0f;
			if (x >= 1f)
				return 1f;

			return (float)Math.Pow(2f, (-10f) * x) * (float)Math.Sin(((x * 10f * magnitude - 0.75f) * c4)) + 1f;
		}
	}

	public struct PPMath
	{
		public static float RandomFloat(float min, float max)
		{
			Random random = new Random();
			return (float)(random.NextDouble() * (max - min) + min);
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
				return min;
			else if (value > max)
				return max;
			return value;
		}
	}
}
