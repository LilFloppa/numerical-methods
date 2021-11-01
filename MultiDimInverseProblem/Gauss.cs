using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiDimInverseProblem
{
    public static class Gauss
    {
        public static double[] Solve(double[,] A, double[] B)
        {
			int N = B.Length;
			double[] x = new double[N];

			for (int k = 0; k < N - 1; k++)
			{
				double max = Math.Abs(A[k, k]);
				int m = k;
				for (int i = k + 1; i < N; i++)
					if (Math.Abs(A[k, i]) > max)
					{
						max = Math.Abs(A[k, i]);
						m = i;
					}

				(B[m], B[k]) = (B[k], B[m]);

				for (int j = k; j < N; j++)
					(A[k, j], A[m, j]) = (A[m, j], A[k, j]);

				for (int i = k + 1; i < N; i++)
				{
					double t = A[i, k] / A[k, k];
					B[i] -= t * B[k];
					for (int j = k + 1; j < N; j++)
						A[i, j] -= t * A[k, j];
				}
			}

			x[N - 1] = B[N - 1] / A[N - 1, N - 1];
			for (int k = N - 2; k >= 0; k--)
			{
				double sum = 0;
				for (int j = k + 1; j < N; j++)
					sum += A[k, j] * x[j];

				x[k] = (B[k] - sum) / A[k, k];
			}

			return x;
		}
    }
}
