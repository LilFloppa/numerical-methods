using MathUtilities;
using System.Collections.Generic;

namespace OrderHarmonization.Meshes
{
    public class Mesh
	{
		public int NodeCount { get; set; }
		public Point[] Points { get; set; }
		public List<FiniteElement> Elements { get; set; }
		public List<FirstBoundaryEdge> FirstBoundary { get; set; }
		public List<SecondBoundaryEdge> SecondBoundary { get; set; }
		public List<ThirdBoundaryEdge> ThirdBoundary { get; set; }
	}
}
