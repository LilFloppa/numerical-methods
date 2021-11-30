using System;

namespace PotOfWater
{
    public class Edge
    {
        public int[] Vertices { get; set; }

        public Func<double, double> F { get; set; }

        public Edge(int EdgeNodeCount) => Vertices = new int[EdgeNodeCount];
    }
}
