﻿using PotOfWater.Meshes;
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
                RoCp = 2.0,
                F = (double x, double y, double t) => 20.0 * x * t + 4
            }
        };

        public static Dictionary<int, Func<double, double, double, double>> FirstBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [0] = (double x, double y, double t) => 5 * x * t * t + 2 * t
        };

        public static Dictionary<int, Func<double, double, double, double>> SecondBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [0] = (double x, double y, double t) => 0.0,
        };

        public static Dictionary<int, (double beta, Func<double, double, double, double> ubeta)> ThirdBoundary => new Dictionary<int, (double, Func<double, double, double, double>)>
        {
            [0] = (0.0, (double x, double y, double t) => 0.0)
        };
    };

    public class ProblemInfo
    {
        public ITwoDimBasis Basis { get; set; }
        public IOneDimBasis BoundaryBasis { get; set; }
        public Mesh Mesh { get; set; }
        public double[] TimeMesh { get; set; }
        public Dictionary<int, Material> MaterialDictionary { get; set; }
        public Dictionary<int, Func<double, double, double, double>> FirstBoundaryDictionary { get; set; }
        public Dictionary<int, Func<double, double, double, double>> SecondBoundaryDictionary { get; set; }
        public Dictionary<int, (double beta, Func<double, double, double, double> ubeta)> ThirdBoundaryDictionary { get; set; }
    }
}
