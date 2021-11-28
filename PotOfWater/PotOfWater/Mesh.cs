using MathUtilities;
using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public class Mesh
	{
		public int NodeCount { get; set; }
		public Point[] Points { get; set; }
		public List<FiniteElement> Elements { get; set; }

		public static Mesh FromFile()
		{
			throw new NotImplementedException();
		}
	}
}
