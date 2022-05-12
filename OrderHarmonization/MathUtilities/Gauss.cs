using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtilities
{
    public static class Gauss
    {
        public static void Solve(double[,] A, double[] x, double[] b)
        {
			int N = x.Length;
			for (int k = 0; k < N - 1; k++)
			{
				// Поиск ведущего элемента
				double max = Math.Abs(A[k, k]);
				int m = k;
				for (int i = k + 1; i < N; i++)
					if (Math.Abs(A[k, i]) > max)
					{
						max = Math.Abs(A[k, i]);
						m = i;
					}

				(b[m], b[k]) = (b[k], b[m]);
				for (int j = k; j < N; j++)
					(A[k, j], A[m, j]) = (A[m, j], A[k, j]);

				// Обнуление k-ого столбца
				for (int i = k + 1; i < N; i++)
				{
					double t = A[i, k] / A[k, k];
					b[i] -= t * b[k];
					for (int j = k + 1; j < N; j++)
						A[i, j] -= t * A[k, j];
				}
			}

			// Вычисление вектора x
			x[N - 1] = b[N - 1] / A[N - 1, N - 1];
			for (int k = N - 2; k >= 0; k--)
			{
				double sum = 0;
				for (int j = k + 1; j < N; j++)
					sum += A[k, j] * x[j];

				x[k] = (b[k] - sum) / A[k, k];
			}
		}
    }
}
