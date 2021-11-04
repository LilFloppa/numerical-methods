using MathUtilities;
using System.Numerics;

namespace MultiDimInverseProblem
{
    public class Receiver
	{
		public Point M { get; set; }
		public Point N { get; set; }
		public Receiver(Point M, Point N)
		{
			this.M = M;
			this.N = N;
		}
	}
}
