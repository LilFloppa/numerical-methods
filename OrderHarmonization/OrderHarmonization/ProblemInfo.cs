using OrderHarmonization.Meshes;
using System;
using System.Collections.Generic;

namespace OrderHarmonization
{
    public static class AreaInfo
    {
        public static Dictionary<int, Material> Materials => new Dictionary<int, Material>
        {
            [0] = new Material
            {
                Name = "Steel",
                Lambda = 1.0,
                RoCp = 1.0,
                F = (double r, double z) => -Math.Exp(r - 8.0) / r
            },
        };

        public static Dictionary<int, Func<double, double, double>> FirstBoundary => new Dictionary<int, Func<double, double, double>>
        {
            [0] = (x, y) => Math.Exp(1.0 - 8.0),
            [1] = (x, y) => Math.Exp(10.0 - 8.0),
        };

        public static Dictionary<int, Func<double, double, double>> SecondBoundary => new Dictionary<int, Func<double, double, double>>
        {
            [0] = (x, y) => 0.0,
        };

        public static Dictionary<int, (double beta, Func<double,  double, double> ubeta)> ThirdBoundary => new Dictionary<int, (double, Func<double, double, double>)>
        {
        };
    }

    public class ProblemInfo
    {
        public ITwoDimBasis Basis { get; set; }
        public IOneDimBasis BoundaryBasis { get; set; }
        public IOneDimBasis BoundaryBasisFirstOrder { get; set; }
        public IOneDimBasis BoundaryBasisThirdOrder { get; set; }
        public Mesh Mesh { get; set; }
        public Dictionary<int, Material> MaterialDictionary { get; set; }
        public Dictionary<int, Func<double, double, double>> FirstBoundaryDictionary { get; set; }
        public Dictionary<int, Func<double, double, double>> SecondBoundaryDictionary { get; set; }
        public Dictionary<int, (double beta, Func<double, double, double> ubeta)> ThirdBoundaryDictionary { get; set; }
    }
}
