using System;

namespace PotOfWater
{
    public class Edge
    {
        public double[] Vertices { get; set; }

        public Func<double, double, double> F { get; set; }
    }
}
