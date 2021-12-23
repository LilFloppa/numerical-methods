using PotOfWater.Meshes;
using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public static class AreaInfo
    {
        public static Dictionary<int, Material> Materials => new Dictionary<int, Material>
        {
            [2] = new Material
            {
                Name = "Steel",
                Lambda = 25.0,
                RoCp = 7800 * 462,
                F = (double r, double z, double t) => 0.0
            },
            [3] = new Material
            {
                Name = "Water",
                Lambda = 0.648,
                RoCp = 1000.0 * 4200.0,
                F = (double r, double z, double t) => 0.0
            }
        };

        public static Dictionary<int, Func<double, double, double, double>> FirstBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [0] = (double r, double z, double t) => 0.0
        };

        public static Dictionary<int, Func<double, double, double, double>> SecondBoundary => new Dictionary<int, Func<double, double, double, double>>
        {
            [2] = (double r, double z, double t) => 1100,
            [3] = (double r, double z, double t) => 0.0,
        };

        public static Dictionary<int, (double beta, Func<double, double, double, double> ubeta)> ThirdBoundary => new Dictionary<int, (double, Func<double, double, double, double>)>
        {
            [0] = (2.0, (double r, double z, double t) => r * r)
        };

        public static (double, double) V(double r, double z)
        {
            double hor = 0.01;
            double ver = hor * 0.0212;
            Func<double, double> increase = (double r) => 0.06 / (0.085 - 1.0e-6) * r + 0.002;
            Func<double, double> decrease = (double r) => -0.06 / (0.085 - 1.0e-6) * r + 0.062;

            if (z > increase(r) && z > decrease(r))
            {
                // top triangle
                (double r, double z) v = (0.0, 0.0);
                v.r = -ver / r;

                double zbeg = 0.032;
                double zmid = 0.047;
                double zend = 0.062;

                if (z > zmid)
                {
                    double c = (z - zmid) / (zend - zmid);
                    v.r *= 1.0 - c;
                }
                else
                {
                    double c = (z - zbeg) / (zmid - zbeg);
                    v.r *= c;
                }

                return v;
            }
            else if (z < increase(r) && z > decrease(r))
            {
                // right triangle
                (double r, double z) v = (0.0, 0.0);
                v.z = hor;

                double rbeg = 0.0424995;
                double rmid = 0.06374975;
                double rend = 0.085;

                if (r > rmid)
                {
                    double c = (r - rmid) / (rend - rmid);
                    v.z *= 1.0 - c;
                }
                else
                {
                    double c = (r - rbeg) / (rmid - rbeg);
                    v.z *= c;
                }

                return v;
            }
            else if (z > increase(r) && z < decrease(r))
            {
                // left triangle
                (double r, double z) v = (0.0, 0.0);
                v.z = -hor;

                double rbeg = 1.0e-6;
                double rmid = 0.02124975;
                double rend = 0.0424995;

                if (r > rmid)
                {
                    double c = (r - rmid) / (rend - rmid);
                    v.z *= 1.0 - c;
                }
                else
                {
                    double c = (r - rbeg) / (rmid - rbeg);
                    v.z *= c;
                }

                return v;
            }
            else if (z < increase(r) && z < decrease(r))
            {
                // bottom triangle
                (double r, double z) v = (0.0, 0.0);
                v.r = ver / r;

                double zbeg = 0.002;
                double zmid = 0.017;
                double zend = 0.032;

                if (z > zmid)
                {
                    double c = (z - zmid) / (zend - zmid);
                    v.r *= 1.0 - c;
                }
                else
                {
                    double c = (z - zbeg) / (zmid - zbeg);
                    v.r *= c;
                }

                return v;
            }

            throw new Exception("Bad point");
        }
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
