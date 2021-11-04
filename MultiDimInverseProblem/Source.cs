using FEM;
using MathUtilities;
using System.Numerics;

namespace MultiDimInverseProblem
{
    public class Source
	{
		public Point A { get; set; }
		public double I { get; set; }

		private FEMrz solution { get; set; }

		public Source(Point A, double I, FEMrz solution)
		{
			this.A = A;
			this.I = I;
			this.solution = solution;
		}

		public double GetValue(Receiver r)
        {
			double AM = Point.Distance(A, r.M);
			double AN = Point.Distance(A, r.N);

			double AMv = solution.U(new Point(AM, 0.0));
			double ANv = solution.U(new Point(AN, 0.0));

			return I * (AMv - ANv);
		}
	}
}
