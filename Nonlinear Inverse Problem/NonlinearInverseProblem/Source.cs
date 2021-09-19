using System;
using System.Numerics;

namespace NonlinearInverseProblem
{
	public class Source
	{
		public Vector2 A { get; set; }
		public Vector2 B { get; set; }
		public double I { get; set; }

		public Source(Vector2 A, Vector2 B, double I)
		{
			this.A = A;
			this.B = B;
			this.I = I;
		}
	}
}
