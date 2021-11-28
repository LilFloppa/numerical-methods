using MathUtilities;
using System;

namespace PotOfWater
{
    public class SLAEBuilder
	{
		private ProblemInfo info;

		private Point[] points;

		private double[,] local;
		private double[] localb;

		public SLAEBuilder(ProblemInfo info)
		{
			this.info = info;

			points = info.Mesh.Points;

			local = new double[info.Basis.Size, info.Basis.Size];
			localb = new double[info.Basis.Size];
		}

		public void Build(IMatrix A, double[] b)
		{
			foreach (FiniteElement e in info.Mesh.Elements)
			{
				ClearLocals();
				BuildLocalMatrix(e);
				BuildLocalB(e);
				AddLocalToGlobal(A, b, e);
			}
		}

		private void ClearLocals()
		{
			for (int i = 0; i < info.Basis.Size; i++)
				for (int j = 0; j < info.Basis.Size; j++)
					local[i, j] = 0.0;

			Array.Fill(localb, 0.0);
		}

		private void BuildLocalMatrix(FiniteElement e)
		{
			// TODO: implement this method
			Point a = points[e[0]];
			Point b = points[e[1]];
			Point c = points[e[2]];

			double D = Math.Abs(Utilities.Det(a, b, c));

			for (int i = 0; i < info.Basis.Size; i++)
				for (int j = 0; j < info.Basis.Size; j++)
				{
					double M = 0.0;
					double G = 0.0;
					local[i, j] = (e.Material.RoCp * M + e.Material.Lambda * G) * D;
				}
		}

		private void BuildLocalB(FiniteElement e)
		{
			// TODO: implement this method

			Point a = points[e[0]];
			Point b = points[e[1]];
			Point c = points[e[2]];

			double D = Math.Abs(Utilities.Det(a, b, c));

			for (int i = 0; i < info.Basis.Size; i++)
			{
				localb[i] = 0.0;
				localb[i] *= D;
			}
		}

		private void AddLocalToGlobal(IMatrix A, double[] B, FiniteElement e)
		{
			var IA = A.Portrait.IA;
			var JA = A.Portrait.JA;

			for (int i = 0; i < info.Basis.Size; i++)
			{
				A.DI[e.Vertices[i]] += local[i, i];
				B[e.Vertices[i]] += localb[i];

				for (int j = 0; j < i; j++)
				{
					int a = e.Vertices[i];
					int b = e.Vertices[j];
					if (a < b) (a, b) = (b, a);

					if (A.Portrait.IA[a + 1] > IA[a])
					{
						Span<int> span = new Span<int>(JA, IA[a], IA[a + 1] - IA[a]);

						// TODO: test Binary Search
						// int k = MemoryExtensions.BinarySearch(span, b);
						int k;
						for (k = 0; k < IA[a + 1] - IA[a]; k++)
							if (span[k] == b)
								break;

						int index = IA[a] + k;
						A.AL[index] += local[i, j];
					}
				}
			}
		}
	}
}
