using System;

namespace MathUtilities
{
	public static class Utilities
	{
		public static double Distance(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

		public static double DotProduct(double[] a, double[] b)
		{
			if (a.Length != b.Length)
				throw new Exception("vectors have different length");

			double result = 0.0;
			int N = a.Length;


			for (int i = 0; i < N; i++)
				result += a[i] * b[i];

			return result;
		}

		public static int Factorial(int n)
		{
			int result = 1;
			for (int i = 2; i <= n; i++)
				result *= i;
			return result;
		}

		public static double Det(Point a, Point b, Point c)
		{
			double x1 = a.X;
			double y1 = a.Y;
			double x2 = b.X;
			double y2 = b.Y;
			double x3 = c.X;
			double y3 = c.Y;

			return (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
		}

		public static double[,] Invert2x2Matrix(double[,] A)
		{
			double[,] result = new double[2, 2];

			double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
			result[0, 0] = A[1, 1] / det;
			result[0, 1] = -A[0, 1] / det;
			result[1, 0] = -A[1, 0] / det;
			result[1, 1] = A[0, 0] / det;

			return result;
		}

		public static double[] Gauss(double[][] A, double[] b)
		{
			int N = A.Length;
			double[] x = new double[N];

			for (int k = 0; k < N - 1; k++)
			{
				// Поиск ведущего элемента
				double max = Math.Abs(A[k][k]);
				int m = k;
				for (int i = k + 1; i < N; i++)
					if (Math.Abs(A[k][i]) > max)
					{
						max = Math.Abs(A[k][i]);
						m = i;
					}

				// Обмен местами b[m] и b[k]
				(b[m], b[k]) = (b[k], b[m]);
				// Обмен местами k-ого и m-ого столбцов
				for (int j = k; j < N; j++)
					(A[k][j], A[m][j]) = (A[m][j], A[k][j]);

				// Обнуление k-ого столбца
				for (int i = k + 1; i < N; i++)
				{
					double t = A[i][k] / A[k][k];
					b[i] -= t * b[k];
					for (int j = k + 1; j < N; j++)
						A[i][j] -= t * A[k][j];
				}
			}

			// Вычисление вектора x
			x[N - 1] = b[N - 1] / A[N - 1][N - 1];
			for (int k = N - 2; k >= 0; k--)
			{
				double sum = 0;
				for (int j = k + 1; j < N; j++)
					sum += A[k][j] * x[j];

				x[k] = (b[k] - sum) / A[k][k];
			}

			return x;
		}

		private static double sign(Point p, Point t1, Point t2)
		{
			return (p.X - t2.X) * (t1.Y - t2.Y) - (t1.X - t2.X) * (p.Y - t2.Y);
		}

		public static bool PointInTriangle(Point t1, Point t2, Point t3, Point p)
		{
			double D = Math.Abs(sign(t1, t2, t3));
			double D1 = Math.Abs(sign(p, t2, t3));
			double D2 = Math.Abs(sign(t1, p, t3));
			double D3 = Math.Abs(sign(t1, t2, p));

			if (Math.Abs(D1 + D2 + D3 - D) > 1.0e-9)
				return false;

			return true;
		}

		public static double[] GetL(Point a, Point b, Point c, Point p)
		{
			double D = Math.Abs(Det(a, b, c));
			double D1 = Math.Abs(Det(p, b, c));
			double D2 = Math.Abs(Det(a, p, c));
			double D3 = Math.Abs(Det(a, b, p));

			double L1 = D1 / D;
			double L2 = D2 / D;
			double L3 = D3 / D;
			return new double[3] { L1, L2, L3 };
		}

		public static double Error(double[] a, double[] b)
		{
			double result = 0.0;
			int N = a.Length;

			for (int i = 0; i < N; i++)
				result += (a[i] - b[i]) * (a[i] - b[i]);

			return Math.Sqrt(result);
		}

		public static Point[] TriangleCubicPoints(Point a, Point b, Point c)
		{
			Point[] points = new Point[10];

			points[0] = a;
			points[1] = b;
			points[2] = c;
			points[3] = (((b / 2.0) + a) * 2.0 / 3.0);
			points[4] = (((b * 2.0) + a) / 3.0);
			points[5] = (((c / 2.0) + a) * 2.0 / 3.0);
			points[6] = (((c * 2.0) + a) / 3.0);
			points[7] = (((c / 2.0) + b) * 2.0 / 3.0);
			points[8] = (((c * 2.0) + b) / 3.0);
			points[9] = ((a + b + c) / 3.0);

			return points;
		}
	}
}
