﻿namespace PotOfWater
{
    public class FiniteElement
	{
		public int[] Vertices { get; set; }
		public Material Material { get; set; }
		public int this[int i] => Vertices[i];

		public FiniteElement(int basisSize) => Vertices = new int[basisSize];
	}
}
