using PotOfWater.Meshes;
using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public static class AreaInfo
    {
        public static Dictionary<int, Material> Materials => new Dictionary<int, Material>
        {
            [0] = new Material
            {
                Lambda = 1.0,
                RoCp = 1.0,
                F = (double x, double y) => x + y
            }
        };

        public static Dictionary<int, Func<double, double>> FirstBoundary => new Dictionary<int, Func<double, double>>
        {
            [0] = (double x) => 0
        };

        public static Dictionary<int, Func<double, double>> SecondBoundary => new Dictionary<int, Func<double, double>>
        {
            [0] = (double x) => 0
        };
    };

    public class ProblemInfo
    {
        public IBasis Basis { get; set; }
        public Mesh Mesh { get; set; }
        public Dictionary<int, Material> MaterialDictionary { get; set; }
        public Dictionary<int, Func<double, double>> FirstBoundaryDictionary { get; set; }
        public Dictionary<int, Func<double, double>> SecondBoundaryDictionary { get; set; }
    }
}
