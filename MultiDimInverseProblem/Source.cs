using FEM;
using MathUtilities;
using System.Numerics;

namespace MultiDimInverseProblem
{
    public class Source
	{
		public Vector2 A { get; set; }
		public Vector2 B { get; set; }
		public double I { get; set; }

		private FEMrz solution { get; set; }

		public Source(Vector2 A, Vector2 B, double I, FEMrz solution)
		{
			this.A = A;
			this.B = B;
			this.I = I;
			this.solution = solution;
		}

		public double GetValue(Receiver r)
        {
			double AM = solution.U(new Point(r.M.X - A.X, r.M.Y));
			double BM = solution.U(new Point(r.M.X - B.X, r.M.Y));
			double ABM = I * (BM - AM);

			double AN = solution.U(new Point(r.N.X - A.X, r.M.Y));
			double BN = solution.U(new Point(r.N.X - B.X, r.M.Y));
			double ABN = I * (BN - AN);

			return ABN - ABM;
		}
	}
}
