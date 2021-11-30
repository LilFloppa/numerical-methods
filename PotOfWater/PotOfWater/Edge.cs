using System;

namespace PotOfWater
{
    public class Edge
    {
        public int NodeCount { get; set; }
        public int[] Vertices { get; set; }
        // TODO: this function should be two-dimensional
        public Func<double, double> F { get; set; }
        public int this[int i] => Vertices[i];

        public Edge(int nodeCount)
        {
            NodeCount = nodeCount;
            Vertices = new int[nodeCount];
        }
    }
}
