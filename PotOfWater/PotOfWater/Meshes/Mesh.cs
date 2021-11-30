using MathUtilities;
using System.Collections.Generic;

namespace PotOfWater.Meshes
{
    public class Mesh
	{
		public int NodeCount { get; set; }
		public Point[] Points { get; set; }
		public List<FiniteElement> Elements { get; set; }
		public List<Edge> FirstBoundary { get; set; }
		public List<Edge> SecondBoundary { get; set; }
		public List<Edge> ThirdBoundary { get; set; }
	}
}
