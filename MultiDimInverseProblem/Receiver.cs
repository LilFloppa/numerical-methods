using System.Numerics;

namespace MultiDimInverseProblem
{
    public class Receiver
	{
		public Vector2 M { get; set; }
		public Vector2 N { get; set; }
		public Receiver(Vector2 M, Vector2 N)
		{
			this.M = M;
			this.N = N;
		}
	}
}
