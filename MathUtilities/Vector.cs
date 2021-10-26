using System;

namespace MathUtilities
{
    public class Vector
	{
		public static double Dot(double[] a, double[] b)
		{
			if (a.Length != b.Length)
				throw new Exception("vectors have different length");

			double result = 0.0;
			int N = a.Length;

			for (int i = 0; i < N; i++)
				result += a[i] * b[i];

			return result;
		}

		public static double Error(double[] a, double[] b)
		{
			double result = 0.0;
			int N = a.Length;

			for (int i = 0; i < N; i++)
				result += (a[i] - b[i]) * (a[i] - b[i]);

			return Math.Sqrt(result);
		}
	}
}