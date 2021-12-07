using System;

namespace PotOfWater
{
	public interface ILayer
    {
		public int Size { get; }
		public double[][] Q { get; }
		public double[] T { get; }
		public double Coeff0 { get; }
		public double[] Coeffs { get; }

		public void SetT(double[] t);
		public void SetQ(double[][] q);
	}

    public class TwoLayer : ILayer
    {
		public int Size => 1;
        public double[][] Q { get; private set; }
        public double[] T { get; private set; }
		public double Coeff0 { get; private set; }
		public double[] Coeffs { get; private set; } = new double[1];

		public void SetT(double[] t)
		{
			if (t.Length != Size + 1)
				throw new Exception();

			T = t;

			Coeff0 = 1.0 / (t[1] - t[0]);
			Coeffs[0] = 1.0 / (t[1] - t[0]);
		}
		public void SetQ(double[][] q)
		{
			if (q.Length != Size)
				throw new Exception();

			Q = q;
		}
    }

	public class ThreeLayer : ILayer
	{
		public int Size => 2;
		public double[][] Q { get; private set; }
		public double[] T { get; private set; }
		public double Coeff0 { get; private set; }
		public double[] Coeffs { get; private set; } = new double[2];

		public void SetT(double[] t)
		{
			if (t.Length != Size + 1)
				throw new Exception();

			T = t;

			double dt = t[2] - t[0];
			double dt0 = t[2] - t[1];
			double dt1 = t[1] - t[0];

			Coeff0 = (dt + dt0) / (dt * dt0);

			Coeffs[0] = -dt0 / (dt * dt1);
			Coeffs[1] = dt / (dt1 * dt0);
		}
		public void SetQ(double[][] q)
		{
			if (q.Length != Size)
				throw new Exception();

			Q = q;
		}
	}

	public class FourLayer : ILayer
	{
		public int Size => 3;
		public double[][] Q { get; private set; }
		public double[] T { get; private set; }
		public double Coeff0 { get; private set; }
		public double[] Coeffs { get; private set; } = new double[3];

		public void SetT(double[] t)
		{
			if (t.Length != Size + 1)
				throw new Exception();

			T = t;

			double dt03 = t[3] - t[0];
			double dt02 = t[3] - t[1];
			double dt01 = t[3] - t[2];

			double dt13 = t[2] - t[0];
			double dt12 = t[2] - t[1];

			double dt23 = t[1] - t[0];

			Coeff0 = (dt02 * dt01 + dt03 * dt01 + dt03 * dt02) / (dt03 * dt02 * dt01);

			Coeffs[0] = dt02 * dt01 / (dt23 * dt13 * dt03);
			Coeffs[1] = -dt03 * dt01 / (dt23 * dt12 * dt02);
			Coeffs[2] = dt03 * dt02 / (dt13 * dt12 * dt01);
		}
		public void SetQ(double[][] q)
		{
			if (q.Length != Size)
				throw new Exception();

			Q = q;
		}
	}
}
