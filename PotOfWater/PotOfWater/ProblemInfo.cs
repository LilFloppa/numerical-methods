using System.Collections.Generic;

namespace PotOfWater
{
    public class ProblemInfo
    {
        public IBasis Basis { get; set; }
        public Mesh Mesh { get; set; }
        public List<Edge> FirstBoundary { get; set; }
        public List<Edge> SecondBoundary { get; set; }
    }
}
