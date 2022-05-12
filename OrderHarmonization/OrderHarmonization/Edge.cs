using System;

namespace OrderHarmonization
{
    public class Edge
    {
        public int NodeCount { get; set; }
        public int[] Vertices { get; set; }
        public int this[int i] => Vertices[i];

        public Edge(int nodeCount)
        {
            NodeCount = nodeCount;
            Vertices = new int[nodeCount];
        }
    }

    public class FirstBoundaryEdge : Edge
    {
        public Func<double, double, double> F { get; set; }

        public FirstBoundaryEdge(int nodeCount) : base(nodeCount) { }
    }

    public class SecondBoundaryEdge : Edge
    {
        public Func<double, double, double> Theta { get; set; }

        public SecondBoundaryEdge(int nodeCount) : base(nodeCount) { }
    }

    public class ThirdBoundaryEdge : Edge
    {
        public double Beta { get; set; }
        public Func<double, double, double> UBeta { get; set; }

        public ThirdBoundaryEdge(int nodeCount) : base(nodeCount) { }
    }
}
