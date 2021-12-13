﻿using PotOfWater.Meshes;
using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public static class AreaInfo
    {
        //public static Dictionary<int, Material> Materials => new Dictionary<int, Material>
        //{
        //    [0] = new Material
        //    {
        //        Lambda = 1.0,
        //        RoCp = 1.0,
        //        F = (double r, double z, double t) => -(2.0 + 2.0) + r * r + z
        //    }
        //};

        //public static Dictionary<int, Func<double, double, double, double>> FirstBoundary => new Dictionary<int, Func<double, double, double, double>>
        //{
        //    [0] = (double r, double z, double t) => r * r + z
        //};

        //public static Dictionary<int, Func<double, double, double, double>> SecondBoundary => new Dictionary<int, Func<double, double, double, double>>
        //{
        //    [0] = (double r, double z, double t) => -1.0
        //};

        //public static Dictionary<int, (double beta, Func<double, double, double, double> ubeta)> ThirdBoundary => new Dictionary<int, (double, Func<double, double, double, double>)>
        //{
        //    [0] = (2.0, (double r, double z, double t) => 6 + z),
        //    [1] = (1.0, (double r, double z, double t) => r * r + 3)
        //};

        public static Dictionary<int, Material> Materials => new Dictionary<int, Material>
        {
            [0] = new Material
            {
                Lambda = 1.0,
                RoCp = 1.0,
                F = (double r, double z, double t) => -4.0 + r * r
            }
        };

        public static Dictionary<int, Func<double, double, double, double>> FirstBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [0] = (double r, double z, double t) => r * r
        };

        public static Dictionary<int, Func<double, double, double, double>> SecondBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [0] = (double r, double z, double t) => -2.0,
            [1] = (double r, double z, double t) => 4.0,
        };

        public static Dictionary<int, (double beta, Func<double, double, double, double> ubeta)> ThirdBoundary => new Dictionary<int, (double, Func<double, double, double, double>)>
        {
            [0] = (2.0, (double r, double z, double t) => r * r)
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
