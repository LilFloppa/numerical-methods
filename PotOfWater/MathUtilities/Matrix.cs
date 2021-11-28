using System;
using System.Linq;

namespace MathUtilities
{
    public interface IMatrix
	{
		public int N { get; set; }
		public double[] DI { get; set; }
		public double[] AL { get; set; }
		public double[] AU { get; set; }
		public Portrait Portrait { get; set; }

		public void Set(int i, int j, double value);
		public void Add(int i, int j, double value);
		public void Multiply(double[] vector, double[] result);
	}

	public class SparseMatrix : IMatrix
	{
		public int N { get; set; } = 0;
		public Portrait Portrait { get; set; } = null;
		public double[] DI { get; set; } = null;
		public double[] AL { get; set; } = null;
		public double[] AU { get; set; } = null;

		public SparseMatrix(int N)
		{
			this.N = N;
			DI = new double[N];
		}

		public void SetPortrait(Portrait portrait)
        {
			Portrait = portrait;
			AL = new double[portrait.IA.Last()];
			AU = new double[portrait.IA.Last()];
        }

		public void Set(int i, int j, double value)
		{
			if (i >= N || j >= N || i < 0 || j < 0)
				throw new ArgumentOutOfRangeException("Bad index for matrix");

			if (i == j)
				DI[i] = value;

			if (j > i)
			{
				int width = Portrait.IA[j + 1] - Portrait.IA[j];
				int begin = j - width;
				int end = j;

				for (int k = begin; k < end; k++)
					if (k == i)
						AU[Portrait.IA[j] + k - begin] = value;
			}
			else
			{
				int width = Portrait.IA[i + 1] - Portrait.IA[i];
				int begin = i - width;
				int end = i;

				for (int k = begin; k < end; k++)
					if (k == j)
						AL[Portrait.IA[i] + k - begin] = value;
			}

			throw new ArgumentOutOfRangeException("Bad index for matrix");
		}

		public void Add(int i, int j, double value)
		{
			if (i >= N || j >= N || i < 0 || j < 0)
				throw new ArgumentOutOfRangeException("Bad index for matrix");

			if (i == j)
				DI[i] = value;

			if (j > i)
			{
				int width = Portrait.IA[j + 1] - Portrait.IA[j];
				int begin = j - width;
				int end = j;

				for (int k = begin; k < end; k++)
					if (k == i)
						AU[Portrait.IA[j] + k - begin] += value;
			}
			else
			{
				int width = Portrait.IA[i + 1] - Portrait.IA[i];
				int begin = i - width;
				int end = i;

				for (int k = begin; k < end; k++)
					if (k == j)
						AL[Portrait.IA[i] + k - begin] += value;
			}

			throw new ArgumentOutOfRangeException("Bad index for matrix");
		}

		public void Multiply(double[] vector, double[] result)
		{
			if (vector.Length != N)
				throw new Exception("Vector's length is not equal Matrix's size");

			for (int i = 0; i < N; i++)
			{
				result[i] = vector[i] * DI[i];
				for (int k = Portrait.IA[i]; k < Portrait.IA[i + 1]; k++)
				{
					int j = Portrait.JA[k];
					result[i] += AL[k] * vector[j];
					result[j] += AU[k] * vector[i];
				}
			}
		}
	}
}
