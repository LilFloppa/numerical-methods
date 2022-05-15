using System;

namespace OrderHarmonization
{
    public class FiniteElement
	{
		public int[] Vertices { get; set; }
		public int Order { get; set; } = 1;
		public int VerticesCount { get; set; } = 0;
		public Material Material { get; set; } = null;
		public int this[int i] => Vertices[i];

		public FiniteElement(int basisSize)
		{
			Vertices = new int[basisSize];
			Array.Fill(Vertices, -1);
		}
	}
}
