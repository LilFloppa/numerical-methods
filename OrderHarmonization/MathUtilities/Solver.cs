using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtilities
{
	public enum SolverTypes { CGMLU, LOSLU, BCGLU }

	public interface ISolver
	{
		public int MaxIterCount { get; set; }
		public int IterCount { get; set; }
		public double Eps { get; set; }
		public double Difference { get; set; }

		public double[] Solve(IMatrix matrix, double[] b);
	}

	struct RawMatrix
	{
		public int N { get; set; }
		public double[] DI { get; set; }
		public double[] AL { get; set; }
		public double[] AU { get; set; }
		public int[] IA { get; set; }
		public int[] JA { get; set; }
	}

	public class LOSLU : ISolver
	{
		public int MaxIterCount { get; set; } = 10000;
		public int IterCount { get; set; } = 0;

		public double Eps { get; set; } = 1.0e-15;
		public double Difference { get; set; } = 0.0;

		int N { get; set; } = 0;

		double[] Ax { get; set; } = null;
		double[] r { get; set; } = null;
		double[] z { get; set; } = null;
		double[] p { get; set; } = null;
		double[] temp { get; set; } = null;
		double[] xPrev { get; set; } = null;

		RawMatrix LU { get; set; }

		public double[] Solve(IMatrix matrix, double[] B)
		{
			IterCount = 0;
			N = matrix.N;
			InitAuxVectors(N);
			LUFactorization(matrix);

			double[] x = new double[N];

			// Calculate r0
			matrix.Multiply(x, Ax);
			for (int i = 0; i < N; i++)
				r[i] = B[i] - Ax[i];

			Forward(LU, r, r);

			//Calculate z0
			Backward(LU, z, r);

			// Calculate p0
			matrix.Multiply(z, p);
			Forward(LU, p, p);

			Difference = Utilities.DotProduct(r, r);

			while (IterCount < MaxIterCount && Difference >= Eps && Utilities.Error(x, xPrev) >= 1.0e-10 * 1.0e-10)
			{
				// Calculate alpha
				double dotP = Utilities.DotProduct(p, p);
				double a = Utilities.DotProduct(p, r) / dotP;

				// Calculate xk, rk
				for (int i = 0; i < N; i++)
				{
					xPrev[i] = x[i];
					x[i] += a * z[i];
					r[i] -= a * p[i];
				}

				// Calculate beta
				Backward(LU, Ax, r);
				matrix.Multiply(Ax, temp);
				Forward(LU, Ax, temp);
				double b = -Utilities.DotProduct(p, Ax) / dotP;

				// Calculate zk, pk
				Backward(LU, temp, r);
				for (int i = 0; i < N; i++)
				{
					z[i] = temp[i] + b * z[i];
					p[i] = Ax[i] + b * p[i];
				}

				// Calculate difference
				Difference = Utilities.DotProduct(r, r);
				IterCount++;
			}

			return x;
		}

		void LUFactorization(IMatrix matrix)
		{
			var IA = matrix.Portrait.IA;
			var JA = matrix.Portrait.JA;
			RawMatrix LU = new RawMatrix();
			LU.N = matrix.N;
			LU.IA = new int[LU.N + 1];

			for (int i = 0; i < matrix.N + 1; i++)
				LU.IA[i] =IA[i];

			LU.AL = new double[LU.IA[LU.N]];
			LU.AU = new double[LU.IA[LU.N]];
			LU.JA = new int[LU.IA[LU.N]];
			LU.DI = new double[LU.N];

			for (int i = 0; i < IA[matrix.N]; i++)
				LU.JA[i] =JA[i];

			for (int i = 0; i < matrix.N; i++)
			{
				double sumD = 0;
				int i0 = IA[i], i1 = IA[i + 1];

				for (int k = i0; k < i1; k++)
				{
					double sumL = 0, sumU = 0;
					int j = JA[k];

					// Calculate L[i][j], U[j][i]
					int j0 = IA[j], j1 = IA[j + 1];

					int kl = i0, ku = j0;

					for (; kl < i1 && ku < j1;)
					{
						int j_kl = JA[kl];
						int j_ku = JA[ku];

						if (j_kl == j_ku)
						{
							sumL += LU.AL[kl] * LU.AU[ku];
							sumU += LU.AU[kl] * LU.AL[ku];
							kl++;
							ku++;
						}
						if (j_kl > j_ku)
							ku++;
						if (j_kl < j_ku)
							kl++;
					}

					LU.AL[k] = matrix.AL[k] - sumL;
					LU.AU[k] = matrix.AU[k] - sumU;
					LU.AU[k] /= LU.DI[j];

					// Calculate sum for DI[i]
					sumD += LU.AL[k] * LU.AU[k];
				}

				// Calculate DI[i]
				LU.DI[i] = matrix.DI[i] - sumD;
			}

			this.LU = LU;
		}

		void InitAuxVectors(int N)
		{
			Ax = new double[N];
			r = new double[N];
			z = new double[N];
			p = new double[N];
			temp = new double[N];
			xPrev = new double[N];

			for (int i = 0; i < N; i++)
				xPrev[i] = 1.0;
		}

		void Forward(RawMatrix A, double[] x, double[] b)
		{
			double[] di = A.DI;
			double[] al = A.AL;
			double[] au = A.AU;
			int[] ia = A.IA;
			int[] ja = A.JA;
			int N = A.N;


			for (int i = 0; i < N; i++)
			{
				double sum = 0;
				int i0 = ia[i], i1 = ia[i + 1];
				for (int k = i0; k < i1; k++)
				{
					int j = ja[k];
					sum += al[k] * x[j];
				}
				x[i] = (b[i] - sum) / di[i];
			}
		}

		void Backward(RawMatrix A, double[] x, double[] b)
		{
			double[] di = A.DI;
			double[] al = A.AL;
			double[] au = A.AU;
			int[] ia = A.IA;
			int[] ja = A.JA;
			int N = A.N;

			for (int i = 0; i < N; i++)
				x[i] = b[i];

			for (int i = N - 1; i >= 0; i--)
			{
				int i0 = ia[i], i1 = ia[i + 1];
				for (int k = i0; k < i1; k++)
				{
					int j = ja[k];
					x[j] -= au[k] * x[i];

				}
			}
		}
	}

	public class BCGLU : ISolver
	{
		public int MaxIterCount { get; set; } = 10000;
		public int IterCount { get; set; } = 0;

		public double Eps { get; set; } = 1.0e-15;
		public double Difference { get; set; } = 0.0;

		int N { get; set; } = 0;

		double[] Ax { get; set; } = null;
		double[] r0 { get; set; } = null;
		double[] r { get; set; } = null;
		double[] z { get; set; } = null;
		double[] p { get; set; } = null;
		double[] y { get; set; } = null;
		double[] Ap { get; set; } = null;
		double[] Az { get; set; } = null;
		double[] xPrev { get; set; } = null;

		RawMatrix LU { get; set; }

		public double[] Solve(IMatrix matrix, double[] B)
		{
			IterCount = 0;
			N = matrix.N;
			InitAuxVectors(N);
			LUFactorization(matrix);

			double[] x = new double[N];


			// r(0)
			matrix.Multiply(x, Ax);
			for (int i = 0; i < N; i++)
				r0[i] = B[i] - Ax[i];
			Forward(LU, r0, r0);

			// z(0)
			Backward(LU, z, r0);

			// r = r(0)
			for (int i = 0; i < N; i++)
				r[i] = r0[i];

			// diff
			double diff = Utilities.DotProduct(r0, r0);

			int k = 0;
			for (; k < MaxIterCount && diff >= Eps; k++)
			{
				// L(-1) * A * U(-1) * z(k - 1)
				Backward(LU, Ax, z);
				matrix.Multiply(Ax, Az);
				Forward(LU, Az, Az);

				// alpha(k)
				double dotP = Utilities.DotProduct(r, r0);
				double a = dotP / Utilities.DotProduct(Az, r0);

				// p(k)
				for (int i = 0; i < N; i++)
					p[i] = r[i] - a * Az[i];

				// L(-1) * A * U(-1) * p(k)
				Backward(LU, Ax, p);
				matrix.Multiply(Ax, Ap);
				Forward(LU, Ap, Ap);

				// gamma(k)
				double g = Utilities.DotProduct(p, Ap) / Utilities.DotProduct(Ap, Ap);

				// y(k)
				for (int i = 0; i < N; i++)
					y[i] = y[i] + a * z[i] + g * p[i];

				// r(k)
				for (int i = 0; i < N; i++)
					r[i] = p[i] - g * Ap[i];

				// beta(k)
				double b = a * Utilities.DotProduct(r, r0) / (g * dotP);

				// z(k)
				for (int i = 0; i < N; i++)
					z[i] = r[i] + b * z[i] - b * g * Az[i];

				// diff
				diff = Utilities.DotProduct(r, r);
			}

			// solution
			Backward(LU, x, y);
			return x;
		}

		void LUFactorization(IMatrix matrix)
		{
			var IA = matrix.Portrait.IA;
			var JA = matrix.Portrait.JA;
			RawMatrix LU = new RawMatrix();
			LU.N = matrix.N;
			LU.IA = new int[LU.N + 1];

			for (int i = 0; i < matrix.N + 1; i++)
				LU.IA[i] = IA[i];

			LU.AL = new double[LU.IA[LU.N]];
			LU.AU = new double[LU.IA[LU.N]];
			LU.JA = new int[LU.IA[LU.N]];
			LU.DI = new double[LU.N];

			for (int i = 0; i < IA[matrix.N]; i++)
				LU.JA[i] = JA[i];

			for (int i = 0; i < matrix.N; i++)
			{
				double sumD = 0;
				int i0 = IA[i], i1 = IA[i + 1];

				for (int k = i0; k < i1; k++)
				{
					double sumL = 0, sumU = 0;
					int j = JA[k];

					// Calculate L[i][j], U[j][i]
					int j0 = IA[j], j1 = IA[j + 1];

					int kl = i0, ku = j0;

					for (; kl < i1 && ku < j1;)
					{
						int j_kl = JA[kl];
						int j_ku = JA[ku];

						if (j_kl == j_ku)
						{
							sumL += LU.AL[kl] * LU.AU[ku];
							sumU += LU.AU[kl] * LU.AL[ku];
							kl++;
							ku++;
						}
						if (j_kl > j_ku)
							ku++;
						if (j_kl < j_ku)
							kl++;
					}

					LU.AL[k] = matrix.AL[k] - sumL;
					LU.AU[k] = matrix.AU[k] - sumU;
					LU.AU[k] /= LU.DI[j];

					// Calculate sum for DI[i]
					sumD += LU.AL[k] * LU.AU[k];
				}

				// Calculate DI[i]
				LU.DI[i] = matrix.DI[i] - sumD;
			}

			this.LU = LU;
		}

		void InitAuxVectors(int N)
		{
			Ax = new double[N];
			r0 = new double[N];
			r = new double[N];
			z = new double[N];
			p = new double[N];
			y = new double[N];
			Ap = new double[N];
			Az = new double[N];
			xPrev = new double[N];

			for (int i = 0; i < N; i++)
				xPrev[i] = 1.0;
		}

		void Forward(RawMatrix A, double[] x, double[] b)
		{
			double[] di = A.DI;
			double[] al = A.AL;
			double[] au = A.AU;
			int[] ia = A.IA;
			int[] ja = A.JA;
			int N = A.N;


			for (int i = 0; i < N; i++)
			{
				double sum = 0;
				int i0 = ia[i], i1 = ia[i + 1];
				for (int k = i0; k < i1; k++)
				{
					int j = ja[k];
					sum += al[k] * x[j];
				}
				x[i] = (b[i] - sum) / di[i];
			}
		}

		void Backward(RawMatrix A, double[] x, double[] b)
		{
			double[] di = A.DI;
			double[] al = A.AL;
			double[] au = A.AU;
			int[] ia = A.IA;
			int[] ja = A.JA;
			int N = A.N;

			for (int i = 0; i < N; i++)
				x[i] = b[i];

			for (int i = N - 1; i >= 0; i--)
			{
				int i0 = ia[i], i1 = ia[i + 1];
				for (int k = i0; k < i1; k++)
				{
					int j = ja[k];
					x[j] -= au[k] * x[i];

				}
			}
		}
	}
}
